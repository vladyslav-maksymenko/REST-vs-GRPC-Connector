using DataConnectorLibraryProject.Enums;

namespace DataConnectorLibraryProject.ServiceInterfaces
{
    public interface IBaseServiceOperations<TEntityDto, in TInputEntityDto> 
        where TEntityDto : class
        where TInputEntityDto : class
    {
        Task<OperationResultWithMetrics<TEntityDto>> GetEntityByIdAsync(string id);
        Task<OperationResultWithMetrics<TEntityDto>> AddEntityAsync(TInputEntityDto entity, DbContextType dbType);
        Task<OperationResultWithMetrics<TEntityDto>> UpdateEntityAsync(string id, TInputEntityDto entity);
        Task<OperationResultWithMetrics<TEntityDto>> DeleteEntityAsync(string id, DbContextType dbType);
        Task<OperationResultWithMetrics<IReadOnlyCollection<TEntityDto>>> GetAllEntitiesAsync();
    }
}