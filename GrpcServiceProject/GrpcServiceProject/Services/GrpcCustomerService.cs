using DataConnectorLibraryProject.Models.ModelsDto;
using DataConnectorLibraryProject.ServiceInterfaces;
using Grpc.Core;
using WebApiProject.Protos;

namespace GrpcServiceProject.Services;

public class GrpcCustomerService : CustomerService.CustomerServiceBase
{
    private readonly ICustomerService customerService;

    public GrpcCustomerService(ICustomerService customerService)
    {
        this.customerService = customerService;
    }

    public override async Task<GetAllCustomersResponse> GetAllCustomers(EmptyRequest request, ServerCallContext context)
    {
        var serviceResponse = await customerService.GetAllEntitiesAsync();
        var response = new GetAllCustomersResponse
        {
            Metrics = new ExecutionMetricsGrpc
            {
                SqlQueryTime = serviceResponse.Metrics?.SqlQueryTime ?? string.Empty,
                MongoQueryTime = serviceResponse.Metrics?.MongoQueryTime ?? string.Empty,
                TotalExecutionTime = serviceResponse.Metrics?.TotalExecutionTime
            }
        };
        response.Customers.AddRange(serviceResponse.Result?.Select(c => new GrpcCustomerDto
        {
            Id = c.Id,
            CustomerName = c.CustomerName,
            EdpouCode = c.EdpouCode,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Patronymic = c.Patronymic,
            PositionId = c.PositionId
        }));
        return response;
    }

    public override async Task<GetCustomerByIdResponse> GetCustomerById(GetCustomerByIdRequest request, ServerCallContext context)
    {
        var serviceResponse = await customerService.GetEntityByIdAsync(request.Id);
        if (!serviceResponse.IsSuccess)
        {
            throw new RpcException(new Status(StatusCode.NotFound, string.Join(", ", serviceResponse.ErrorMessages)));
        }

        return new GetCustomerByIdResponse
        {
            Customer = new GrpcCustomerDto
            {
                Id = serviceResponse.Result?.Id,
                CustomerName = serviceResponse.Result?.CustomerName,
                EdpouCode = serviceResponse.Result?.EdpouCode,
                FirstName = serviceResponse.Result?.FirstName,
                LastName = serviceResponse.Result?.LastName,
                Patronymic = serviceResponse.Result?.Patronymic,
                PositionId = serviceResponse.Result?.PositionId
            },
            Metrics = new ExecutionMetricsGrpc
            {
                SqlQueryTime = serviceResponse.Metrics?.SqlQueryTime ?? string.Empty,
                MongoQueryTime = serviceResponse.Metrics?.MongoQueryTime ?? string.Empty,
                TotalExecutionTime = serviceResponse.Metrics?.TotalExecutionTime
            }
        };
    }
    
    public override async Task<AddCustomerResponse> AddCustomer(AddCustomerRequest request, ServerCallContext context)
    {
        var serviceResponse = await customerService.AddEntityAsync(new CustomerInputDto
        {
            CustomerName = request.Customer.CustomerName,
            EdpouCode = request.Customer.EdpouCode,
            FirstName = request.Customer.FirstName,
            LastName = request.Customer.LastName,
            Patronymic = request.Customer.Patronymic,
            PositionId = request.Customer.PositionId
        }, ConvertDbContextType(request.DbType));
        
        return new AddCustomerResponse
        {
            Customer = new GrpcCustomerDto
            {
                Id = serviceResponse.Result?.Id,
                CustomerName = serviceResponse.Result?.CustomerName,
                EdpouCode = serviceResponse.Result?.EdpouCode,
                FirstName = serviceResponse.Result?.FirstName,
                LastName = serviceResponse.Result?.LastName,
                Patronymic = serviceResponse.Result?.Patronymic,
                PositionId = serviceResponse.Result?.PositionId
            },
            Metrics = new ExecutionMetricsGrpc
            {
                SqlQueryTime = serviceResponse.Metrics?.SqlQueryTime ?? string.Empty,
                MongoQueryTime = serviceResponse.Metrics?.MongoQueryTime ?? string.Empty,
                TotalExecutionTime = serviceResponse.Metrics?.TotalExecutionTime
            }
        };
    }
    
    public override async Task<UpdateCustomerResponse> UpdateCustomer(UpdateCustomerRequest request, ServerCallContext context)
    {
        var serviceResponse = await customerService.UpdateEntityAsync(request.Id, new CustomerInputDto
        {
            CustomerName = request.Customer.CustomerName,
            EdpouCode = request.Customer.EdpouCode,
            FirstName = request.Customer.FirstName,
            LastName = request.Customer.LastName,
            Patronymic = request.Customer.Patronymic,
            PositionId = request.Customer.PositionId
        });
        if (!serviceResponse.IsSuccess)
        {
            throw new RpcException(new Status(StatusCode.NotFound, string.Join(", ", serviceResponse.ErrorMessages)));
        }
        return new UpdateCustomerResponse
        {
            Metrics = new ExecutionMetricsGrpc
            {
                SqlQueryTime = serviceResponse.Metrics?.SqlQueryTime ?? string.Empty,
                MongoQueryTime = serviceResponse.Metrics?.MongoQueryTime ?? string.Empty,
                TotalExecutionTime = serviceResponse.Metrics?.TotalExecutionTime
            }
        };
    }
    public override async Task<DeleteCustomerResponse> DeleteCustomer(DeleteCustomerRequest request, ServerCallContext context)
    {
        var serviceResponse = await customerService.DeleteEntityAsync(request.Id, ConvertDbContextType(request.DbType));
        if (!serviceResponse.IsSuccess)
        {
            throw new RpcException(new Status(StatusCode.NotFound, string.Join(", ", serviceResponse.ErrorMessages)));
        }
        return new DeleteCustomerResponse
        {
            Metrics = new ExecutionMetricsGrpc
            {
                SqlQueryTime = serviceResponse.Metrics?.SqlQueryTime ?? string.Empty,
                MongoQueryTime = serviceResponse.Metrics?.MongoQueryTime ?? string.Empty,
                TotalExecutionTime = serviceResponse.Metrics?.TotalExecutionTime
            }
        };
    }
    
    private static DataConnectorLibraryProject.Enums.DbContextType ConvertDbContextType(DbContextType grpcDbType)
    {
        return grpcDbType switch
        {
            DbContextType.Sql => DataConnectorLibraryProject.Enums.DbContextType.Sql,
            DbContextType.Mongo => DataConnectorLibraryProject.Enums.DbContextType.Mongo,
            _ => throw new ArgumentOutOfRangeException(nameof(grpcDbType), "Unsupported DbContextType")
        };
    }
}