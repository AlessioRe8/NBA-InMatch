using System.Text.Json.Serialization;

namespace progetto_cloud.Models.Common
{
    public class ResponseWrapper<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; }

        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }
    }

    public class Meta
    {
        [JsonPropertyName("next_cursor")]
        public int? NextCursor { get; set; }

        [JsonPropertyName("prev_cursor")]
        public int? PreviousCursor { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }
    }
}
