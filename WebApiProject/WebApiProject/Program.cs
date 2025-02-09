using DataConnectorLibraryProject.Extensions;
using WebApiProject.ExtendSwagerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => //Ignore null properties to JSON response.
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.SchemaFilter<EnumSchemaFilter>(); });

builder.Services.AddDataConnectors(builder.Configuration);
builder.Services.AddCustomMapping();

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