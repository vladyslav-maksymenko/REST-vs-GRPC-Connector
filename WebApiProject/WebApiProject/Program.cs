using Microsoft.EntityFrameworkCore;
using DataConnectorLibraryProject.Interface;
using DataConnectorLibraryProject.UnitOfWork;
using DataConnectorLibraryProject.DataAccess.Data;
using DataConnectorLibraryProject.Serializers;
using WebApiProject.ExtendSwager;
using DataConnectorLibraryProject.Settings.Mongo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => //Ignore null properties to JSON response.
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.SchemaFilter<EnumSchemaFilter>(); });

// SQL Server Configuration
builder.Services.AddDbContext<SqlDataConnectorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionForSqlDb")));
//
MongoDbSerialization.AddCustomMongoDbSerialization();
var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));


builder.Services.AddDbContext<MongoDataConnectorDbContext>(options =>
    options.UseMongoDB(mongoSettings.AtlasUri ?? "", mongoSettings.DatabaseName ?? ""));


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();