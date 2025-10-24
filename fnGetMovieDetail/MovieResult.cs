using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace fnGetAllMovies
{
    public class movieResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("director")]
        public string Director { get; set; }

        [JsonProperty("releaseYear")]
        public int ReleaseYear { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }
    }
}