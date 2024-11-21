using AutoMapper;
using DataConnectorLibraryProject.DatabaseStrategy;
using DataConnectorLibraryProject.Interface;
using DataConnectorLibraryProject.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Diagnostics;
using WebApiProject.ExtendSwager;
using WebApiProject.ModelsDTO;

namespace WebApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestfullApiController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public RestfullApiController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet("get-all-customers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCustomers()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            unitOfWork.SwitchContext(unitOfWork.DbContexts.Sql());

            var sqlStopwatch = new Stopwatch();

            var customerSqlRepository = unitOfWork.GetRepository<Customer>();
            sqlStopwatch.Start();
            var customersFromSqlDb = await customerSqlRepository.GetAllAsync();
            sqlStopwatch.Stop();

            unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());

            var mongoStopwatch = new Stopwatch();

            var customerMongoRepository = unitOfWork.GetRepository<Customer>();

            mongoStopwatch.Start();
            var customersFromMongo = await customerMongoRepository.GetAllAsync();
            mongoStopwatch.Stop();


            var resultCustomerList = customersFromMongo.Concat(customersFromSqlDb).Select(x => mapper.Map<Customer, CustomerDTO>(x)).ToArray();

            stopwatch.Stop();

            var response = new ResponseDTO<IReadOnlyCollection<CustomerDTO>>(
                resultCustomerList,
                new ExecutionMetrics(FormatElapsedTime(sqlStopwatch.ElapsedMilliseconds),
                FormatElapsedTime(mongoStopwatch.ElapsedMilliseconds),
                FormatElapsedTime(stopwatch.ElapsedMilliseconds)));

            return Ok(response);
        }

        [HttpGet("get-customer-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();


            unitOfWork.SwitchContext(unitOfWork.DbContexts.Sql());
            var sqlStopwatch = new Stopwatch();

            var customerRepository = unitOfWork.GetRepository<Customer>();

            sqlStopwatch.Start();
            var customersFromSqlDb = await customerRepository.GetByIdAsync(id);
            sqlStopwatch.Stop();

            if(customersFromSqlDb != null)
            {
                var customerDto = mapper.Map<Customer, CustomerDTO>(customersFromSqlDb);
                stopwatch.Stop();

                return Ok(new ResponseDTO<CustomerDTO>(
                   customerDto,
                   Metrics: new ExecutionMetrics(
                       FormatElapsedTime(sqlStopwatch.ElapsedMilliseconds),
                       null,
                       FormatElapsedTime(stopwatch.ElapsedMilliseconds)
                   )));
            }

            var mongoStopwatch = new Stopwatch();

            unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());
            var customerMongoRepository = unitOfWork.GetRepository<Customer>();
            mongoStopwatch.Start();
            var customerFromMongo = await customerMongoRepository.GetByIdAsync(id);
            mongoStopwatch.Stop();

            var customerDtoFromMongo = mapper.Map<Customer, CustomerDTO>(customerFromMongo);
            stopwatch.Stop();

            if (customerFromMongo == null)
            {
                return NotFound(new { Message = $"Customer with ID '{id}' was not found in either SQL or MongoDB." });
            }

            return Ok(new ResponseDTO<CustomerDTO>(
                   customerDtoFromMongo,
                   Metrics: new ExecutionMetrics(
                       null,
                       FormatElapsedTime(mongoStopwatch.ElapsedMilliseconds),
                       FormatElapsedTime(stopwatch.ElapsedMilliseconds)
                   )));
        }

        [HttpPost("add-customer/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerInputDTO customerInputData,[FromQuery] DbType dbType = DbType.Sql)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            if (customerInputData == null)
            {
               return BadRequest(new { Message = "Customer data is required." });
            }

            var customerInputForDb = mapper.Map<Customer>(customerInputData);
            customerInputForDb.Id = dbType == DbType.Mongo
               ? ObjectId.GenerateNewId().ToString()
               : Guid.NewGuid().ToString();

            unitOfWork.SwitchContext(IndicateContext(dbType));
            var customerRepository = unitOfWork.GetRepository<Customer>();

            await customerRepository.AddAsync(customerInputForDb);

            await unitOfWork.SaveShangesAsync();

            stopwatch.Stop();
            return Ok(customerInputForDb.Id);
        }

        [HttpPut("update-customer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomerById(string id, [FromBody] CustomerInputDTO customerInputData)
        {
            if (customerInputData == null)
            {
                return BadRequest(new { Message = "Customer data is required." });
            }

            var mapCustomer = mapper.Map<Customer>(customerInputData);

            unitOfWork.SwitchContext(IndicateContext());

            var customerRepository = unitOfWork.GetRepository<Customer>();
            var customerFromSqlDb = await customerRepository.GetByIdAsync(id);

            unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());
            var customerMongoRepository = unitOfWork.GetRepository<Customer>();
            var customerFromMongoDb = await customerMongoRepository.GetByIdAsync(id);

            if (customerFromSqlDb == null && customerFromMongoDb == null)
            {
                return NotFound(new { Message = "Customer not found." });
            }


            if (customerFromSqlDb != null)
            {
                var updated =  mapper.Map<CustomerInputDTO, Customer>(customerInputData, customerFromSqlDb);
                await customerRepository.UpdateAsync(updated);
                await unitOfWork.SaveShangesAsync();
            }

            if (customerFromMongoDb != null)
            {
                unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());
                var updated = mapper.Map<CustomerInputDTO, Customer>(customerInputData, customerFromMongoDb);
                await customerMongoRepository.UpdateAsync(customerFromMongoDb);
                await unitOfWork.SaveShangesAsync();
            }

            return Ok(new { Message = "Customer updated successfully." });
        }

        [HttpDelete("delete-customer/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustomerById(string id, [FromQuery] DbType dbType = DbType.Sql)
        {
            unitOfWork.SwitchContext(IndicateContext(dbType));

            var customerRepository = unitOfWork.GetRepository<Customer>();

            var getCustomerFromDatabase= await customerRepository.GetByIdAsync(id);

            if (getCustomerFromDatabase == null)
            {
                return NotFound(new { Message = "Customer not found." });
            }

            await customerRepository.DeleteAsync(getCustomerFromDatabase.Id);
            await unitOfWork.SaveShangesAsync();

            return NoContent();
        }

        private IDatabaseStrategy IndicateContext(DbType dbType = DbType.Sql)
        {
            return dbType switch
            {
                DbType.Sql => unitOfWork.DbContexts.Sql(),
                DbType.Mongo => unitOfWork.DbContexts.Mongo(),
                _ => throw new ArgumentException($"Unsupported database type: {dbType}", nameof(dbType))
            };
        }

        private string FormatElapsedTime(long milliseconds)
        {
            var time = TimeSpan.FromMilliseconds(milliseconds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
        }
    }
}
