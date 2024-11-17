using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using DataConnectorLibraryProject.Interface;
using DataConnectorLibraryProject.UnitOfWork;
using DataConnectorLibraryProject.DataAccess.Data;
using WebApiProject.Controllers;
using Microsoft.OpenApi.Models;
using System.Data;
using Microsoft.OpenApi.Any;
using WebApiProject.ModelsDTO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<EnumSchemaFilter>();
});

// SQL Server Configuration
builder.Services.AddDbContext<SqlDataConnectorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionForSqlDb")));
//
var mongoSettings = builder.Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
//var mongoSettings = builder.Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();

builder.Services.AddDbContext<MongoDataConnectorDbContext>(options =>
    options.UseMongoDB(mongoSettings.AtlaURI ?? "", mongoSettings.DatabaseName ?? "")); 


/*builder.Services.AddDbContext<MongoDataConnectorDbContext>(options =>
{
    // Use the service provider to access the required settings or services
    var serviceProvider = builder.Services.BuildServiceProvider();
    var mongoSettings = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value;

    var client = new MongoClient(mongoSettings.AtlasURI);
    var database = client.GetDatabase(mongoSettings.DatabaseName);

    options.UseMongoDB(client, database.DatabaseNamespace.DatabaseName);
});*/


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();