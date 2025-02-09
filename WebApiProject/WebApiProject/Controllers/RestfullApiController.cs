using DataConnectorLibraryProject.Models.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using DataConnectorLibraryProject.Enums;
using DataConnectorLibraryProject.ServiceInterfaces;

namespace WebApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestfullApiController : Controller
    {
        private readonly ICustomerService customerService;

        public RestfullApiController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpGet("get-all-customers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCustomers()
        {
            var serviceResponse= await customerService.GetAllEntitiesAsync();
            if (!serviceResponse.IsSuccess)
            {
                return BadRequest(serviceResponse.ErrorMessages);
            }

            return Ok(new
            {
                Customer = serviceResponse.Result, serviceResponse.Metrics
            });
        }

        [HttpGet("get-customer-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            var serviceResponse= await customerService.GetEntityByIdAsync(id);
            if (!serviceResponse.IsSuccess)
            {
                return NotFound(serviceResponse.ErrorMessages);
            }

            return Ok(new
            {
                Customer = serviceResponse.Result, serviceResponse.Metrics
            });
        }
        
        [HttpPost("add-customer/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCustomer(
            [FromBody] CustomerInputDto? customerInputData,[FromQuery] DbContextType dbType = DbContextType.Sql)
        {
            if (customerInputData == null)
            {
               return BadRequest(new { Message = "Customer data is required." });
            }

            var serviceResponse= await customerService.AddEntityAsync(customerInputData, dbType);

            return Ok(new
            {
                Customer = serviceResponse.Result, serviceResponse.Metrics
            });
        }
        
        [HttpPut("update-customer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomerById(string id, [FromBody] CustomerInputDto? customerInputData)
        {
            if (customerInputData == null)
            {
                return BadRequest(new { Message = "Customer data is required." });
            }

            var serviceResponse= await customerService.UpdateEntityAsync(id, customerInputData);
            if (!serviceResponse.IsSuccess)
            {
                return NotFound(serviceResponse.ErrorMessages);
            }

            return Ok(new
            {
                serviceResponse.Metrics
            });
        }
        
        [HttpDelete("delete-customer/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustomerById(string id, [FromQuery] DbContextType dbType = DbContextType.Sql)
        {
            var serviceResponse= await customerService.DeleteEntityAsync(id, dbType);
            if (!serviceResponse.IsSuccess)
            {
                return NotFound(serviceResponse.ErrorMessages);
            }

            return Ok(new
            {
                serviceResponse.Metrics
            });
        }
    }
}
