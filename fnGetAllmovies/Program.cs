using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Cosmos;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        var cosmosDbConnection = Environment.GetEnvironmentVariable("CosmosDBConnection");
        if (string.IsNullOrWhiteSpace(cosmosDbConnection))
            throw new InvalidOperationException("A variável CosmosDBConnection não está configurada.");

        // Adiciona CosmosClient como Singleton
        services.AddSingleton(new CosmosClient(cosmosDbConnection));
    })
    .Build();

host.Run();
