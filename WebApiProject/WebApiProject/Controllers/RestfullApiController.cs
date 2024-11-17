using AutoMapper;
using DataConnectorLibraryProject.DatabaseStrategy;
using DataConnectorLibraryProject.Interface;
using DataConnectorLibraryProject.Models;
using Microsoft.AspNetCore.Mvc;
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
        {/*
            unitOfWork.SwitchContext(unitOfWork.DbContexts.Sql());
            var customerSqlRepository = unitOfWork.GetRepository<Customer>();
            var customersFromSqlDb = await customerSqlRepository.GetAllAsync();*/

            unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());
            var customerMongoRepository = unitOfWork.GetRepository<Customer>();
            var customersFromMongo = await customerMongoRepository.GetAllAsync();

            var result = customersFromMongo;

            return Ok(result);
        }

        [HttpGet("get-customer-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var customerRepository = unitOfWork.GetRepository<Customer>();
            unitOfWork.SwitchContext(unitOfWork.DbContexts.Sql());
            var customersFromSqlDb = await customerRepository.GetByIdAsync(id);

            unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());
            var customersFromMongo = await customerRepository.GetByIdAsync(id);

            return Ok(customersFromSqlDb?? customersFromMongo);
        }

        [HttpPost("add-customer/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerDTO customerInputData,[FromQuery] DbType dbType = DbType.Sql)
        {
           if(customerInputData == null)
           {
              return BadRequest(new { Message = "Customer data is required." });
           }

           var customer = mapper.Map<CustomerDTO, Customer>(customerInputData);

           unitOfWork.SwitchContext(IndicateContext(dbType));
           var customerRepository = unitOfWork.GetRepository<Customer>();

           await customerRepository.AddAsync(customer);

           await unitOfWork.SaveShangesAsync();

           return Ok(customer.Id);
        }

        [HttpPut("update-customer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomerById(Guid id, [FromBody] CustomerDTO customerInputData)
        {
            if (customerInputData == null)
            {
                return BadRequest(new { Message = "Customer data is required." });
            }

            var mapCustomer = mapper.Map<CustomerDTO, Customer>(customerInputData);


            var context = IndicateContext();
            unitOfWork.SwitchContext(context);

            var customerRepository = unitOfWork.GetRepository<Customer>();

            var customerFromSqlDb = await customerRepository.GetByIdAsync(id);

            unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());
            var customerFromMongoDb = await customerRepository.GetByIdAsync(id);

            if (customerFromSqlDb == null && customerFromMongoDb == null)
            {
                return NotFound(new { Message = "Customer not found." });
            }

            if (customerFromSqlDb != null)
            {
                mapper.Map(customerInputData, customerFromSqlDb);
                await customerRepository.UpdateAsync(customerFromSqlDb);
                await unitOfWork.SaveShangesAsync();
            }

            // Если пользователь найден в Mongo, обновляем в Mongo
            if (customerFromMongoDb != null)
            {
                mapper.Map(customerInputData, customerFromMongoDb);  // Автоматическое обновление полей
                unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());
                await customerRepository.UpdateAsync(customerFromMongoDb);
                await unitOfWork.SaveShangesAsync();
            }

            return Ok(new { Message = "Customer updated successfully." });
        }

        [HttpDelete("delete-customer/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustomerById(Guid id, [FromQuery] DbType dbType = DbType.Sql)
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
    }
}
