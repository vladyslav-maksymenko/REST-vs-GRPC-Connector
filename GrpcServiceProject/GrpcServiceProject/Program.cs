using DataConnectorLibraryProject.Extensions;
using GrpcServiceProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDataConnectors(builder.Configuration);
builder.Services.AddCustomMapping();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GrpcCustomerService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();