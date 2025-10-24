using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;
using System;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddSingleton(sp =>
        {
            string? connectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("A string de conexão do Cosmos DB não foi encontrada.");

            return new CosmosClient(connectionString);
        });
    })
    .Build();

host.Run();
