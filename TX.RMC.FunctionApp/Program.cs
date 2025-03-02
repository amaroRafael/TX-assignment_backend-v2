using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TX.RMC.BusinessLogic;
using TX.RMC.DataService.MongoDB;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

var connectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
var dbName = builder.Configuration.GetValue<string>("MongoDBDatabase");
if (!string.IsNullOrWhiteSpace(connectionString) && !string.IsNullOrWhiteSpace(dbName))
{
    builder.Services.AddMongoDbServices(connectionString, dbName);
}

builder.Services.AddBusinessLogicServices();

builder.Build().Run();
