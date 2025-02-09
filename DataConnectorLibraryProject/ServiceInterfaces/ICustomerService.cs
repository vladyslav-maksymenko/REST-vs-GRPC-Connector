using DataConnectorLibraryProject.Models.ModelsDto;

namespace DataConnectorLibraryProject.ServiceInterfaces
{
    public interface ICustomerService : IBaseServiceOperations<CustomerDto, CustomerInputDto>
    {
    }
}