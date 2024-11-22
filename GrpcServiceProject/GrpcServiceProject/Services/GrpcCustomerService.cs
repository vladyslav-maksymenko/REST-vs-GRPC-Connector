using System.Diagnostics;
using AutoMapper;
using DataConnectorLibraryProject.Interface;
using DataConnectorLibraryProject.Models.ModelsDto;
using DataConnectorLibraryProject.Models.ServerSideModels;
using Grpc.Core;
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
}