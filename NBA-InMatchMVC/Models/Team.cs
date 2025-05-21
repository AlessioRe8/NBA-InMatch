using System.Text.Json.Serialization;

namespace progetto_cloud.Models
{
    public class Team
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }
        [JsonPropertyName("abbreviation")]
        public string Abbreviation { get; set; }
        [JsonPropertyName("city")]
        public string City { get; set; }
        [JsonPropertyName("conference")]
        public string Conference { get; set; }
        [JsonPropertyName("division")]
        public string Division { get; set; }
    }
}
