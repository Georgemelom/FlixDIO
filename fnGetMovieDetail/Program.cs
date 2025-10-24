using Microsoft.Cosmos.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);

        builder.ConfigureFunctionsWebApplication();

        //builder.Services
        //   .AddApplicationInsightsTelemetryWorkerService()
        // .ConfigureFunctionsApplicationInsights();
        builder.Services.AddSingleton((s) =>
        {
            var cosmosDbConnection = Environment.GetEnvironmentVariable("CosmosDBConnection");
            return new CosmosClient(cosmosDbConnection);

            builder.Build().Run();
        });
    }
}