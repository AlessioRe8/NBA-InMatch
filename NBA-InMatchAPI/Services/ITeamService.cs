using progetto_cloud.Models;
using progetto_cloud.Models.Common;
using System.Text.Json;

namespace progetto_cloud.Services
{
    public interface ITeamService
    {
        public Task<(List<Team> teams, int? previousCursor, int? nextCursor)> GetTeamsAsync(int cursor = 0, int perPage = 25);
        Task<(List<Team> teams, int? nextCursor, int? prevCursor)> GetTeamsFilteredAsync(int cursor = 0, int perPage = 25, string conference = null, string division = null);
        Task<Team> GetTeamByIdAsync(int id);
    }

    public class TeamService : ITeamService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TeamService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        

        public async Task<(List<Team> teams, int? previousCursor, int? nextCursor)> GetTeamsAsync(int cursor = 0, int perPage = 25)
        {
            var client = _httpClientFactory.CreateClient("balldontlie");

            var url = $"teams?per_page={perPage}&cursor={cursor}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var wrapper = await JsonSerializer.DeserializeAsync<ResponseWrapper<Team>>(
                await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return (
                wrapper?.Data ?? new List<Team>(),
                wrapper?.Meta?.NextCursor,
                wrapper?.Meta?.PreviousCursor
            );
        }

        public async Task<(List<Team> teams, int? nextCursor, int? prevCursor)> GetTeamsFilteredAsync(
        int cursor = 0, int perPage = 25,
        string conference = null, string division = null)
        {

            var qs = new List<string> { $"per_page={perPage}", $"cursor={cursor}" };
            if (!string.IsNullOrWhiteSpace(conference))
                qs.Add($"conference={Uri.EscapeDataString(conference)}");
            if (!string.IsNullOrWhiteSpace(division))
                qs.Add($"division={Uri.EscapeDataString(division)}");

            var url = "teams?" + string.Join("&", qs);
            var resp = await _httpClientFactory.CreateClient("balldontlie").GetAsync(url);
            resp.EnsureSuccessStatusCode();

            var wrapper = await JsonSerializer.DeserializeAsync<ResponseWrapper<Team>>(
                await resp.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return (wrapper?.Data, wrapper?.Meta?.NextCursor, wrapper?.Meta?.PreviousCursor);
        }

        public async Task<Team> GetTeamByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("balldontlie");
            var response = await client.GetAsync($"teams/{id}");
            response.EnsureSuccessStatusCode();

            var single = await JsonSerializer.DeserializeAsync<SingleResponseWrapper<Team>>(
                await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return single?.Data;
        }
    }
}
