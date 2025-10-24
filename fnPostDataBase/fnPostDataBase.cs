using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace fnPostDataBase
{
    public class fnPostDataBase
    {
        private readonly CosmosClient _cosmosClient;
        private readonly ILogger _logger;

        public fnPostDataBase(CosmosClient cosmosClient, ILoggerFactory loggerFactory)
        {
            _cosmosClient = cosmosClient;
            _logger = loggerFactory.CreateLogger<fnPostDataBase>();
        }

        [Function("fnPostDataBase")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var body = await req.ReadAsStringAsync();
            var movie = JsonSerializer.Deserialize<MovieRequest>(body);

            if (movie == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Dados inválidos.");
                return badResponse;
            }

            string databaseName = Environment.GetEnvironmentVariable("DatabaseName") ?? "FlixDioDB";
            string containerName = Environment.GetEnvironmentVariable("ContainerName") ?? "Movies";

            var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            var container = await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            await container.Container.CreateItemAsync(movie, new PartitionKey(movie.Id));

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Filme '{movie.Title}' inserido com sucesso!");
            return response;
        }
    }
}
