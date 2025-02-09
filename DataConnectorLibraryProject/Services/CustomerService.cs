using System.Diagnostics;
using AutoMapper;
using DataConnectorLibraryProject.DatabaseStrategy;
using DataConnectorLibraryProject.Enums;
using DataConnectorLibraryProject.Interface;
using DataConnectorLibraryProject.Models.ModelsDto;
using DataConnectorLibraryProject.Models.ResponseModels;
using DataConnectorLibraryProject.Models.ServerSideModels;
using DataConnectorLibraryProject.ServiceInterfaces;
using MongoDB.Bson;

namespace DataConnectorLibraryProject.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<OperationResultWithMetrics<IReadOnlyCollection<CustomerDto>>> GetAllEntitiesAsync()
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

            var resultCustomerList = customersFromMongo
                .Concat(customersFromSqlDb)
                .Select(x => mapper.Map<Customer, CustomerDto>(x))
                .ToArray();

            totalStopwatch.Stop();

            return OperationResultWithMetrics<IReadOnlyCollection<CustomerDto>>
                .WithDataAndMetrics(
                    resultCustomerList,
                    new ExecutionMetrics(
                        FormatElapsedTime(sqlStopwatch.ElapsedMilliseconds),
                        FormatElapsedTime(mongoStopwatch.ElapsedMilliseconds),
                        FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)));
        }
        
        public async Task<OperationResultWithMetrics<CustomerDto>> GetEntityByIdAsync(string id)
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
        
                return OperationResultWithMetrics<CustomerDto>.WithDataAndMetrics(
                   customerDto,
                   new ExecutionMetrics(
                       FormatElapsedTime(sqlStopwatch.ElapsedMilliseconds),
                       null,
                       FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)
                   ));
            }
        
            var mongoStopwatch = new Stopwatch();
        
            unitOfWork.SwitchContext(unitOfWork.DbContexts.Mongo());
            var customerMongoRepository = unitOfWork.GetRepository<Customer>();
            mongoStopwatch.Start();
            var customerFromMongo = await customerMongoRepository.GetByIdAsync(id);
            mongoStopwatch.Stop();
            totalStopwatch.Stop();
        
            if (customerFromMongo == null)
            {
                return OperationResultWithMetrics<CustomerDto>
                    .Failure($"Customer with ID '{id}' was not found in either SQL or MongoDB.");
            }
            var customerDtoFromMongo = mapper.Map<Customer, CustomerDto>(customerFromMongo);
            
            return OperationResultWithMetrics<CustomerDto>.WithDataAndMetrics(
                customerDtoFromMongo,
                new ExecutionMetrics(
                    null,
                    FormatElapsedTime(mongoStopwatch.ElapsedMilliseconds),
                    FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)
                ));
        }

        public async Task<OperationResultWithMetrics<CustomerDto>> AddEntityAsync(CustomerInputDto entity, DbContextType dbType)
        {
            var totalStopwatch = new Stopwatch();
            totalStopwatch.Start();
            var mappedEntity = mapper.Map<Customer>(entity);
            mappedEntity.Id = dbType == DbContextType.Mongo
                ? ObjectId.GenerateNewId().ToString()
                : Guid.NewGuid().ToString();
        
            unitOfWork.SwitchContext(IndicateContext(dbType));
        
            var dbStopwatch = new Stopwatch();
        
            var customerRepository = unitOfWork.GetRepository<Customer>();
        
            dbStopwatch.Start();
            await customerRepository.AddAsync(mappedEntity);
            dbStopwatch.Stop();
        
            await unitOfWork.SaveShangesAsync();
        
            totalStopwatch.Stop();
            
            return OperationResultWithMetrics<CustomerDto>.WithDataAndMetrics(
                mapper.Map<Customer, CustomerDto>(mappedEntity),
                GetExecutionMetrics(
                    dbType,
                    dbStopwatch.ElapsedMilliseconds,
                    totalStopwatch.ElapsedMilliseconds));
        }

        public async Task<OperationResultWithMetrics<CustomerDto>> UpdateEntityAsync(string id, CustomerInputDto entity)
        {
            var totalStopwatch = new Stopwatch();
            totalStopwatch.Start();
        
            var sqlUpdateMetrics = await UpdateCustomerInDatabase(id, entity, DbContextType.Sql);
        
            var mongoUpdateMetrics = await UpdateCustomerInDatabase(id, entity, DbContextType.Mongo);
        
            totalStopwatch.Stop();
        
            if (sqlUpdateMetrics == null && mongoUpdateMetrics == null)
            {
                return OperationResultWithMetrics<CustomerDto>.Failure("Customer not found.");
            }

            return OperationResultWithMetrics<CustomerDto>.WithDataAndMetrics(
                metrics: new ExecutionMetrics(
                    SqlQueryTime: sqlUpdateMetrics?.SqlQueryTime,
                    MongoQueryTime: mongoUpdateMetrics?.MongoQueryTime,
                    TotalExecutionTime: FormatElapsedTime(totalStopwatch.ElapsedMilliseconds)));
        }

        public async Task<OperationResultWithMetrics<CustomerDto>> DeleteEntityAsync(string id, DbContextType dbType)
        {
            var totalStopwatch = new Stopwatch();
            totalStopwatch.Start();
            unitOfWork.SwitchContext(IndicateContext(dbType));
        
            var customerRepository = unitOfWork.GetRepository<Customer>();
            var getCustomerFromDatabase= await customerRepository.GetByIdAsync(id);
            if (getCustomerFromDatabase == null)
            {
                return OperationResultWithMetrics<CustomerDto>.Failure($"Customer with ID '{id}' was not found in database.");
            }

            var deleteStopwatch = new Stopwatch();
            deleteStopwatch.Start();
            await customerRepository.DeleteAsync(getCustomerFromDatabase.Id);
            deleteStopwatch.Stop();
        
            await unitOfWork.SaveShangesAsync();
        
            totalStopwatch.Stop();
        
            return OperationResultWithMetrics<CustomerDto>.WithDataAndMetrics(
                metrics:GetExecutionMetrics(
                    dbType,
                    deleteStopwatch.ElapsedMilliseconds,
                    totalStopwatch.ElapsedMilliseconds));
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
        
        private ExecutionMetrics GetExecutionMetrics(
            DbContextType dbType,
            long operationTimeMs,
            long totalExecutionTimeMs)
        {
            return dbType switch
            {
                DbContextType.Sql => new ExecutionMetrics(
                    SqlQueryTime: FormatElapsedTime(operationTimeMs),
                    TotalExecutionTime: FormatElapsedTime(totalExecutionTimeMs)),
                DbContextType.Mongo => new ExecutionMetrics(
                    MongoQueryTime: FormatElapsedTime(operationTimeMs),
                    TotalExecutionTime: FormatElapsedTime(totalExecutionTimeMs)),
                _ => throw new ArgumentOutOfRangeException(nameof(dbType), dbType, "Unsupported database type.")
            };
        }
        
        private string FormatElapsedTime(long milliseconds)
        {
            var time = TimeSpan.FromMilliseconds(milliseconds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
        }

        private async Task<ExecutionMetrics?> UpdateCustomerInDatabase(
            string id, CustomerInputDto customer, DbContextType dbType)
        {
            unitOfWork.SwitchContext(IndicateContext(dbType));
            var customerRepository = unitOfWork.GetRepository<Customer>();
            var existingCustomer = await customerRepository.GetByIdAsync(id);
            if (existingCustomer == null)
            {
                return null;
            }
            
            var mappedCustomer = mapper.Map<CustomerInputDto, Customer>(customer, existingCustomer);
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