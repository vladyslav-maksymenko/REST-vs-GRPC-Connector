using AutoMapper;
using DataConnectorLibraryProject.DatabaseStrategy;
using DataConnectorLibraryProject.Interface;
using DataConnectorLibraryProject.Models.ModelsDto;
using DataConnectorLibraryProject.Models.ResponseModels;
using DataConnectorLibraryProject.Models.ServerSideModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Data;
using System.Diagnostics;
using WebApiProject.ExtendSwager;

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
            var totalStopwatch = new Stopwatch();
            totalStopwatch.Start();

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


            var resultCustomerList = customersFromMongo.Concat(customersFromSqlDb).Select(x => mapper.Map<Customer, CustomerDto>(x)).ToArray();

            totalStopwatch.Stop();

            var response = new ResponseDto<IReadOnlyCollection<CustomerDto>>(
                resultCustomerList,
                new ExecutionMetrics(FormatElapsedTime(sqlStopwatch.ElapsedMilliseconds),
                FormatElapsedTime(mongoStopwatch.ElapsedMilliseconds),
                FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)));

            return Ok(response);
        }

        [HttpGet("get-customer-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            var totalStopwatch = new Stopwatch();
            totalStopwatch.Start();


            unitOfWork.SwitchContext(unitOfWork.DbContexts.Sql());
            var sqlStopwatch = new Stopwatch();

            var customerRepository = unitOfWork.GetRepository<Customer>();

            sqlStopwatch.Start();
            var customersFromSqlDb = await customerRepository.GetByIdAsync(id);
            sqlStopwatch.Stop();

            if(customersFromSqlDb != null)
            {
                var customerDto = mapper.Map<Customer, CustomerDto>(customersFromSqlDb);
                totalStopwatch.Stop();

                return Ok(new ResponseDto<CustomerDto>(
                   customerDto,
                   Metrics: new ExecutionMetrics(
                       FormatElapsedTime(sqlStopwatch.ElapsedMilliseconds),
                       null,
                       FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)
                   )));
            }

            var mongoStopwatch = new Stopwatch();

            unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());
            var customerMongoRepository = unitOfWork.GetRepository<Customer>();
            mongoStopwatch.Start();
            var customerFromMongo = await customerMongoRepository.GetByIdAsync(id);
            mongoStopwatch.Stop();

            var customerDtoFromMongo = mapper.Map<Customer, CustomerDto>(customerFromMongo);
            totalStopwatch.Stop();

            if (customerFromMongo == null)
            {
                return NotFound(new { Message = $"Customer with ID '{id}' was not found in either SQL or MongoDB." });
            }

            return Ok(new ResponseDto<CustomerDto>(
                   customerDtoFromMongo,
                   Metrics: new ExecutionMetrics(
                       null,
                       FormatElapsedTime(mongoStopwatch.ElapsedMilliseconds),
                       FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)
                   )));
        }

        [HttpPost("add-customer/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerInputDto customerInputData,[FromQuery] DbContextType dbType = DbContextType.Sql)
        {
            var totalStopwatch = new Stopwatch();
            totalStopwatch.Start();
            if (customerInputData == null)
            {
               return BadRequest(new { Message = "Customer data is required." });
            }

            var customerInputForDb = mapper.Map<Customer>(customerInputData);
            customerInputForDb.Id = dbType == DbContextType.Mongo
               ? ObjectId.GenerateNewId().ToString()
               : Guid.NewGuid().ToString();

            unitOfWork.SwitchContext(IndicateContext(dbType));

            var dbStopwatch = new Stopwatch();

            var customerRepository = unitOfWork.GetRepository<Customer>();

            dbStopwatch.Start();
            await customerRepository.AddAsync(customerInputForDb);
            dbStopwatch.Stop();

            await unitOfWork.SaveShangesAsync();

            totalStopwatch.Stop();

            var metrics = GetExecutionMetrics(dbType, dbStopwatch.ElapsedMilliseconds, totalStopwatch.ElapsedMilliseconds);

            return Ok(new ResponseDto<Customer>(customerInputForDb,Metrics: metrics));
        }

        [HttpPut("update-customer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomerById(string id, [FromBody] CustomerInputDto customerInputData)
        {
            if (customerInputData == null)
            {
                return BadRequest(new { Message = "Customer data is required." });
            }

            var totalStopwatch = new Stopwatch();
            totalStopwatch.Start();


            var sqlUpdateMetrics = await UpdateCustomerInDatabase(id, DbContextType.Sql, customerInputData);

            var mongoUpdateMetrics = await UpdateCustomerInDatabase(id, DbContextType.Mongo, customerInputData);

            totalStopwatch.Stop();

            if (sqlUpdateMetrics == null && mongoUpdateMetrics == null)
            {
                return NotFound(new { Message = "Customer not found." });
            }

            return Ok(new
            {
                Message = "Customer updated successfully.",
                Metrics = new ExecutionMetrics(
                    SqlQueryTime: sqlUpdateMetrics?.SqlQueryTime,
                    MongoQueryTime: mongoUpdateMetrics?.MongoQueryTime,
                    TotalExecutionTime: FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)
                )
            });
        }

        [HttpDelete("delete-customer/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustomerById(string id, [FromQuery] DbContextType dbType = DbContextType.Sql)
        {
            var totalStopwatch = new Stopwatch();
            totalStopwatch.Start();

            unitOfWork.SwitchContext(IndicateContext(dbType));

            var customerRepository = unitOfWork.GetRepository<Customer>();

            var getCustomerFromDatabase= await customerRepository.GetByIdAsync(id);

            if (getCustomerFromDatabase == null)
            {
                return NotFound(new { Message = $"Customer with ID '{id}' not found in {dbType} database." });
            }
            var deleteStopwatch = new Stopwatch();
            deleteStopwatch.Start();
            await customerRepository.DeleteAsync(getCustomerFromDatabase.Id);
            deleteStopwatch.Stop();

            await unitOfWork.SaveShangesAsync();

            totalStopwatch.Stop();
            var metrics = GetExecutionMetrics(dbType, deleteStopwatch.ElapsedMilliseconds, totalStopwatch.ElapsedMilliseconds);

            return Ok(new
            {
                Message = $"Customer with ID '{id}' deleted successfully.",
                Metrics = metrics
            });
        }

        private IDatabaseStrategy IndicateContext(DbContextType dbType = DbContextType.Sql)
        {
            return dbType switch
            {
                DbContextType.Sql => unitOfWork.DbContexts.Sql(),
                DbContextType.Mongo => unitOfWork.DbContexts.Mongo(),
                _ => throw new ArgumentException($"Unsupported database type: {dbType}", nameof(dbType))
            };
        }

        private string FormatElapsedTime(long milliseconds)
        {
            var time = TimeSpan.FromMilliseconds(milliseconds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
        }

        private ExecutionMetrics GetExecutionMetrics(DbContextType dbType, long operationTimeMs, long totalExecutionTimeMs)
        {
           return dbType switch
            {
                DbContextType.Sql => new ExecutionMetrics(SqlQueryTime: FormatElapsedTime(operationTimeMs), TotalExecutionTime: FormatElapsedTime(totalExecutionTimeMs)),
                DbContextType.Mongo => new ExecutionMetrics(MongoQueryTime: FormatElapsedTime(operationTimeMs), TotalExecutionTime: FormatElapsedTime(totalExecutionTimeMs)),
                _ => throw new ArgumentOutOfRangeException(nameof(dbType), dbType, "Unsupported database type.")
            };
        }

        private async Task<ExecutionMetrics?> UpdateCustomerInDatabase(string id, DbContextType dbType, CustomerInputDto customerInputData)
        {
            unitOfWork.SwitchContext(IndicateContext(dbType));
            var customerRepository = unitOfWork.GetRepository<Customer>();
            var existingCustomer = await customerRepository.GetByIdAsync(id);

            if (existingCustomer == null)
            {
                return null;
            }

            var mappedCustomer = mapper.Map<CustomerInputDto, Customer>(customerInputData, existingCustomer);

            var updateStopwatch = new Stopwatch();
            updateStopwatch.Start();
            await customerRepository.UpdateAsync(mappedCustomer);
            updateStopwatch.Stop();

            await unitOfWork.SaveShangesAsync();

            return new ExecutionMetrics(
                SqlQueryTime: dbType == DbContextType.Sql ? FormatElapsedTime(updateStopwatch.ElapsedMilliseconds) : null,
                MongoQueryTime: dbType == DbContextType.Mongo ? FormatElapsedTime(updateStopwatch.ElapsedMilliseconds) : null
            );
        }
    }
}
