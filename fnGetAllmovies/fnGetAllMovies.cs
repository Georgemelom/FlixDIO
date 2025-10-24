using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Cosmos;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

public class FnGetAllMovies
{
    private readonly CosmosClient _cosmosClient;

    public FnGetAllMovies(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    [Function("GetAllMovies")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetAllMovies")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);

        // Acessa o banco e container
        var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync("MoviesDb");
        var container = await database.Database.CreateContainerIfNotExistsAsync(
            id: "Movies",               // nome do container
            partitionKeyPath: "/id");   // sem throughput -> funciona no serverless

        // Consulta todos os itens
        var query = new QueryDefinition("SELECT * FROM c");
        var iterator = container.Container.GetItemQueryIterator<MovieResult>(query);

        var results = new List<MovieResult>();
        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync())
            {
                results.Add(item);
            }
        }

        // Retorna JSON
        await response.WriteAsJsonAsync(results);
        return response;
    }
}
