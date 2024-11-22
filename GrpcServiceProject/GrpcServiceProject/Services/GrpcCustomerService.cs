using System.Diagnostics;
using AutoMapper;
using DataConnectorLibraryProject.DatabaseStrategy;
using DataConnectorLibraryProject.Interface;
using DataConnectorLibraryProject.Models.ModelsDto;
using DataConnectorLibraryProject.Models.ResponseModels;
using DataConnectorLibraryProject.Models.ServerSideModels;
using Grpc.Core;
using MongoDB.Bson;
using WebApiProject.Protos;

namespace GrpcServiceProject.Services;

public class GrpcCustomerService : CustomerService.CustomerServiceBase
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public GrpcCustomerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public override async Task<GetAllCustomersResponse> GetAllCustomers(EmptyRequest request, ServerCallContext context)
    {
        var totalStopwatch = Stopwatch.StartNew();
        
        unitOfWork.SwitchContext(unitOfWork.DbContexts.Sql());
        var sqlStopwatch = Stopwatch.StartNew();
        var customersFromSqlDb = await unitOfWork.GetRepository<Customer>().GetAllAsync();
        sqlStopwatch.Stop(); 
        
        unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());
        var mongoStopwatch = Stopwatch.StartNew();
        var customersFromMongo = await unitOfWork.GetRepository<Customer>().GetAllAsync();
        mongoStopwatch.Stop();


        var customerDtos = customersFromMongo
            .Concat(customersFromSqlDb)
            .Select(mapper.Map<CustomerDto>)
            .ToArray();

        totalStopwatch.Stop();

        return new GetAllCustomersResponse
        {
            Customers = { customerDtos.Select(ToGrpcCustomerDto) },
            Metrics = new ExecutionMetricsGrpc
            {
                SqlQueryTime = FormatElapsedTime(sqlStopwatch.ElapsedMilliseconds),
                MongoQueryTime = FormatElapsedTime(mongoStopwatch.ElapsedMilliseconds),
                TotalExecutionTime = FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)
            }
        };
    }

    public override async Task<GetCustomerByIdResponse> GetCustomerById(GetCustomerByIdRequest request, ServerCallContext context)
    {
        var totalStopwatch = Stopwatch.StartNew();
        
        unitOfWork.SwitchContext(unitOfWork.DbContexts.Sql());
        var sqlStopwatch = Stopwatch.StartNew();
        var customerFromSql = await unitOfWork.GetRepository<Customer>().GetByIdAsync(request.Id);
        sqlStopwatch.Stop();

        if (customerFromSql != null)
        {
            totalStopwatch.Stop();
            return new GetCustomerByIdResponse
            {
                Customer = ToGrpcCustomerDto(mapper.Map<CustomerDto>(customerFromSql)),
                Metrics = new ExecutionMetricsGrpc
                {
                    SqlQueryTime = FormatElapsedTime(sqlStopwatch.ElapsedMilliseconds),
                    TotalExecutionTime = FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)
                }
            };
        }
        
        unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());
        var mongoStopwatch = Stopwatch.StartNew();
        var customerFromMongo = await unitOfWork.GetRepository<Customer>().GetByIdAsync(request.Id);
        mongoStopwatch.Stop();

        if (customerFromMongo == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Customer with ID '{request.Id}' "));
        }

        totalStopwatch.Stop();
        return new GetCustomerByIdResponse
        {
            Customer = ToGrpcCustomerDto(mapper.Map<CustomerDto>(customerFromMongo)),
            Metrics = new ExecutionMetricsGrpc
            {
                MongoQueryTime = FormatElapsedTime(mongoStopwatch.ElapsedMilliseconds),
                TotalExecutionTime = FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)
            }
        };
    }
    
    public override async Task<AddCustomerResponse> AddCustomer(AddCustomerRequest request, ServerCallContext context)
    {
        var totalStopwatch = Stopwatch.StartNew();
        if (request == null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Customer data is required."));
        }
        
        var customer = new Customer
        {
            Id = request.DbType == "mongo"
                ? ObjectId.GenerateNewId().ToString()
                : Guid.NewGuid().ToString(),
            CustomerName = request.Customer.CustomerName,
            EdpouCode =  request.Customer.EdpouCode,
            FirstName =  request.Customer.FirstName,
            LastName =  request.Customer.LastName,
            Patronymic =  request.Customer.Patronymic,
            PositionId =  request.Customer.PositionId,
        };
        
        unitOfWork.SwitchContext(IndicateContext(request.DbType));
        
         var customerRepository = unitOfWork.GetRepository<Customer>();
         var dbStopwatch = Stopwatch.StartNew();
         await customerRepository.AddAsync(customer);
         dbStopwatch.Stop();
        
         await unitOfWork.SaveShangesAsync();

         totalStopwatch.Stop();
         var metrics = GetExecutionMetrics(request.DbType, dbStopwatch.ElapsedMilliseconds, totalStopwatch.ElapsedMilliseconds);
        return new AddCustomerResponse
        {
            Customer = ToGrpcCustomerDto(mapper.Map<CustomerDto>(customer)),
            Metrics = new ExecutionMetricsGrpc
            {
                SqlQueryTime = metrics.SqlQueryTime ?? string.Empty,
                MongoQueryTime = metrics.MongoQueryTime ?? string.Empty,
                TotalExecutionTime = metrics.TotalExecutionTime ?? string.Empty
            }
        };
    }
    
    public override async Task<UpdateCustomerResponse> UpdateCustomerById(UpdateCustomerRequest request, ServerCallContext context)
    {
        if (request.Customer == null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Customer data is required."));
        }
        var customerInputData = new CustomerInputDto()
        {
            CustomerName = request.Customer.CustomerName,
            EdpouCode =  request.Customer.EdpouCode,
            FirstName =  request.Customer.FirstName,
            LastName =  request.Customer.LastName,
            Patronymic =  request.Customer.Patronymic,
            PositionId =  request.Customer.PositionId,
        };

        var totalStopwatch = Stopwatch.StartNew();

        var sqlUpdateMetrics = await UpdateCustomerInDatabaseAsync(request.Id, "sql", customerInputData);
        var mongoUpdateMetrics = await UpdateCustomerInDatabaseAsync(request.Id, "mongo", customerInputData);

        totalStopwatch.Stop();

        if (sqlUpdateMetrics == null && mongoUpdateMetrics == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Customer with ID '{request.Id}' not found in both databases."));
        }

        return new UpdateCustomerResponse
        {
            Message = "Customer updated successfully.",
            Metrics = new ExecutionMetricsGrpc
            {
                SqlQueryTime = sqlUpdateMetrics?.SqlQueryTime ?? string.Empty,
                MongoQueryTime = mongoUpdateMetrics?.MongoQueryTime ?? string.Empty,
                TotalExecutionTime = FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)
            }
        };
    }


    public override async Task<DeleteCustomerResponse> DeleteCustomerById(DeleteCustomerRequest request, ServerCallContext context)
    {
        var totalStopwatch = Stopwatch.StartNew();
        unitOfWork.SwitchContext(IndicateContext(request.DbType));
        
        var customerRepository = unitOfWork.GetRepository<Customer>();
        var getCustomerFromDatabase= await customerRepository.GetByIdAsync(request.Id);
        
        if (getCustomerFromDatabase == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Customer with ID '{request.Id}' not found in {request.DbType} database."));
        }
        
        var deleteStopwatch = Stopwatch.StartNew();
        await customerRepository.DeleteAsync(getCustomerFromDatabase.Id);
        deleteStopwatch.Stop();
        
        await unitOfWork.SaveShangesAsync();

        totalStopwatch.Stop();
        var metrics = GetExecutionMetrics(request.DbType, deleteStopwatch.ElapsedMilliseconds, totalStopwatch.ElapsedMilliseconds);
        return new DeleteCustomerResponse
        {
            Message = $"Customer with ID '{request.Id}' deleted successfully.",
            Metrics = new ExecutionMetricsGrpc
            {
                SqlQueryTime = metrics.SqlQueryTime ?? string.Empty,
                MongoQueryTime = metrics.MongoQueryTime ?? string.Empty,
                TotalExecutionTime = metrics.TotalExecutionTime ?? string.Empty
            }
        };
    }
    
    private IDatabaseStrategy IndicateContext(string dbType)
    {
        return dbType switch
        {
            "sql" => unitOfWork.DbContexts.Sql(),
            "mongo" => unitOfWork.DbContexts.Mongo(),
            _ => throw new ArgumentException($"Unsupported database type: {dbType}", nameof(dbType))
        };
    }
    
    private async Task<ExecutionMetricsGrpc?> UpdateCustomerInDatabaseAsync(string id, string dbType, CustomerInputDto customerInputData)
    {
        unitOfWork.SwitchContext(IndicateContext(dbType));
        var customerRepository = unitOfWork.GetRepository<Customer>();
        var existingCustomer = await customerRepository.GetByIdAsync(id);

        if (existingCustomer == null)
        {
            return null;
        }
        
        var mappedCustomer = mapper.Map<CustomerInputDto, Customer>(customerInputData, existingCustomer);

        var updateStopwatch = Stopwatch.StartNew();
        await customerRepository.UpdateAsync(mappedCustomer);
        updateStopwatch.Stop();

        await unitOfWork.SaveShangesAsync();

        return new ExecutionMetricsGrpc
        {
            SqlQueryTime = dbType == "sql" ? FormatElapsedTime(updateStopwatch.ElapsedMilliseconds) : string.Empty,
            MongoQueryTime = dbType == "mongo" ? FormatElapsedTime(updateStopwatch.ElapsedMilliseconds) : string.Empty
        };
    }


    private string FormatElapsedTime(long milliseconds)
    {
        var time = TimeSpan.FromMilliseconds(milliseconds);
        return $"{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
    }

    private GrpcCustomerDto ToGrpcCustomerDto(CustomerDto dto) =>
        new GrpcCustomerDto
        {
            Id = dto.Id,
            CustomerName = dto.CustomerName,
            EdpouCode = dto.EdpouCode,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Patronymic = dto.Patronymic,
            PositionId = dto.PositionId
        };
    
    private ExecutionMetrics GetExecutionMetrics(string dbType, long operationTimeMs, long totalExecutionTimeMs)
    {
        return dbType switch
        {
            "sql" => new ExecutionMetrics(SqlQueryTime: FormatElapsedTime(operationTimeMs), TotalExecutionTime: FormatElapsedTime(totalExecutionTimeMs)),
            "mongo" => new ExecutionMetrics(MongoQueryTime: FormatElapsedTime(operationTimeMs), TotalExecutionTime: FormatElapsedTime(totalExecutionTimeMs)),
            _ => throw new ArgumentOutOfRangeException(nameof(dbType), dbType, "Unsupported database type.")
        };
    }
}