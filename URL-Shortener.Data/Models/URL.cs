

using System.Text.Json.Serialization;

namespace URL_Shortener.Data.Models
{
    public class URL
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } 

        [JsonPropertyName("shortenedURL")]
        public string ShortenedURL { get; set; }

        [JsonPropertyName("originalURL")]
        public string OriginalURL { get; set; } 

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }   

    }
}
