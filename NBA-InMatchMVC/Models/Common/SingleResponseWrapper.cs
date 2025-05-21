using System.Text.Json.Serialization;

namespace progetto_cloud.Models.Common
{
    public class SingleResponseWrapper<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
    }
}
