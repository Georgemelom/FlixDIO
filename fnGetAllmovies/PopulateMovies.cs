using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
    static async Task Main(string[] args)
    {
        string cosmosDbConnection = Environment.GetEnvironmentVariable("CosmosDBConnection");

        if (string.IsNullOrWhiteSpace(cosmosDbConnection))
        {
            Console.WriteLine("Defina a variável de ambiente CosmosDBConnection com a string de conexão.");
            return;
        }

        var client = new CosmosClient(cosmosDbConnection);

        // Cria ou acessa o banco
        var databaseResponse = await client.CreateDatabaseIfNotExistsAsync("MoviesDb");
        var database = databaseResponse.Database;

        // Cria ou acessa o container (sem throughput, serverless compatível)
        var containerResponse = await database.CreateContainerIfNotExistsAsync(
            id: "Movies",
            partitionKeyPath: "/id"
        );
        var container = containerResponse.Container;

        // Lista de filmes de teste
        var filmes = new List<MovieResult>
        {
            new MovieResult { Id = Guid.NewGuid().ToString(), Title = "Matrix", Director = "Wachowski", Genre = "Sci-Fi" },
            new MovieResult { Id = Guid.NewGuid().ToString(), Title = "Inception", Director = "Christopher Nolan", Genre = "Sci-Fi" },
            new MovieResult { Id = Guid.NewGuid().ToString(), Title = "The Godfather", Director = "Francis Ford Coppola", Genre = "Crime" },
            new MovieResult { Id = Guid.NewGuid().ToString(), Title = "Titanic", Director = "James Cameron", Genre = "Romance" }
        };

        // Insere os filmes
        foreach (var filme in filmes)
        {
            await container.UpsertItemAsync(filme, new PartitionKey(filme.Id));
            Console.WriteLine($"Filme inserido: {filme.Title}");
        }

        Console.WriteLine("Todos os filmes foram adicionados com sucesso!");
    }
}

// Modelo de filme (igual ao da Function)
public class MovieResult
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Director { get; set; } = null!;
    public string Genre { get; set; } = null!;
}
