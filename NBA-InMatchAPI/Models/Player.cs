using System.Text.Json.Serialization;

namespace progetto_cloud.Models
{
    public class Player
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("position")]
        public string Position { get; set; }

        [JsonPropertyName("height")]
        public string Height { get; set; }

        [JsonPropertyName("weight")]
        public string Weight { get; set; }

        [JsonPropertyName("jersey_number")]
        public string JerseyNumber { get; set; }

        [JsonPropertyName("college")]
        public string College { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("draft_year")]
        public int? DraftYear { get; set; }

        [JsonPropertyName("draft_round")]
        public int? DraftRound { get; set; }

        [JsonPropertyName("draft_number")]
        public int? DraftNumber { get; set; }

        [JsonPropertyName("team")]
        public Team Team { get; set; }
    }
}
