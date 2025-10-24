using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace flixdio.Function;

public class HttpTrigger1
{
    private readonly ILogger<HttpTrigger1> _logger;
    private readonly CosmosClient _cosmosClient;

    public HttpTrigger1(ILogger<HttpTrigger1> logger)
    {
        _logger = logger;
        _cosmosClient = cosmosClient;
    }

    [Function("DetailMovie")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var container = _cosmosClient.GetContainer("FlixDioDB", "Movies");
        var Id = req.Query["id"];
        var partitionKey = req.Query["partitionKey"];
        var Query = $"SELECT * FROM c WHERE c.id = @id";
        var queryDefinition = new QueryDefinition(Query).WithParameter.("@"id";Id); 
        var result = container.GetItemQueryIterator<MovieResult>(queryDefinition);
        var results = new List<MovieResult>();
        while (result.HasMoreResults)
        {
            foreach (var item in await result.ReadNextAsync())
            {
                resultss.Add(item);
            }

            var responseMessage = req.CreateResponse(System.Net.FtpStatusCode.OK);
            await responseMessage.WriteAsJsonAsync(results > Firstordefault());
            return ResponseMessage;
        }
    }

}