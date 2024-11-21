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
using DataConnectorLibraryProject.Serializers;
using MongoDB.Bson.Serialization;
using WebApiProject.ExtendSwager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<EnumSchemaFilter>();
});

// SQL Server Configuration
builder.Services.AddDbContext<SqlDataConnectorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionForSqlDb")));
//
MongoDbSerialization.AddCustomMongoDbSerialization();
var mongoSettings = builder.Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));


builder.Services.AddDbContext<MongoDataConnectorDbContext>(options =>
    options.UseMongoDB(mongoSettings.AtlaURI ?? "", mongoSettings.DatabaseName ?? ""));


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