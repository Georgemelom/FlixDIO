using Newtonsoft.Json;

public class MovieResult
{
    [JsonProperty("id")]
    public string Id { get; set; } = null!; // obrigatório

    [JsonProperty("title")]
    public string Title { get; set; } = null!;

    [JsonProperty("director")]
    public string Director { get; set; } = null!;

    [JsonProperty("genre")]
    public string Genre { get; set; } = null!;
}
