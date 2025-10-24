using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FLIXDIO.Function;

public class fnPostDataStorage
{
    private readonly ILogger<fnPostDataStorage> _logger;

    public fnPostDataStorage(ILogger<fnPostDataStorage> logger)
    {
        _logger = logger;
    }

    [Function("DataStorage")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("Processando a Imagem no Storage");

        try
        {
            if (req.Headers.TryGetValue("file-type", out var fileTypeHeader))
            {
                return new BadRequestObjectResult("Cabeçalho {fileType} é obrigatorio");
            }
            var fileType = fileTypeHeader.ToString();
            var Form = await req.ReadFormAsync();
            var file = Form.Files["file"];
            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("Arquivo não encontrado no corpo da requisição");
            }
            // Processar o arquivo conforme necessário
            //string? conectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage") 
                          ?? throw new InvalidOperationException("AzureWebJobsStorage não encontrada");

            string containerName = fileType;
            BlobClient blobClient = new(connectionString, containerName, file.FileName);
            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            string blobName = file.FileName;
            var blob = containerClient.GetBlobClient(blobName);
            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, true);
            }
            _logger.LogInformation($"Arquivo {file.FileName} armazenado com sucesso");

            return new OkObjectResult(new
            {
                message = "Arquivo armazenado com sucesso",
                blobUri = blob.Uri.ToString()

            }
            );

        }   catch (Exception ex)
        {
            _logger.LogError($"Erro ao processar o arquivo: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
       
}