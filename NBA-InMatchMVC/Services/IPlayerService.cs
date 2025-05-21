using progetto_cloud.Models;
using progetto_cloud.Models.Common;
using System.Net.Http;
using System.Text.Json;

namespace progetto_cloud.Services
{
    public interface IPlayerService
    {
        Task<(List<Player> players, int? nextCursor, int? previousCursor)> GetPlayersAsync(int cursor = 0, int perPage = 25);
        Task<(List<Player> players, int? nextCursor)> GetPlayersFilteredAsync(
        int cursor = 0, int perPage = 25,
        string search = null,
        IEnumerable<int> teamIds = null,
        IEnumerable<int> playerIds = null);
        Task<Player> GetPlayerByIdAsync(int id);
        Task<(List<Player> players, int? nextCursor)> GetPlayersByTeamAsync(int teamId, int cursor = 0, int perPage = 25);
    }

    public class PlayerService : IPlayerService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public PlayerService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<(List<Player> players, int? nextCursor, int? previousCursor)> GetPlayersAsync(int cursor = 0, int perPage = 25)
        {
            var client = _httpClientFactory.CreateClient("balldontlie");

            var url = $"players?per_page={perPage}&cursor={cursor}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var wrapper = await JsonSerializer.DeserializeAsync<ResponseWrapper<Player>>(
                await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return (
                wrapper?.Data ?? new List<Player>(),
                wrapper?.Meta?.NextCursor,
                wrapper?.Meta?.PreviousCursor
            );
        }

        public async Task<(List<Player> players, int? nextCursor)> GetPlayersFilteredAsync(
        int cursor = 0, int perPage = 25,
        string search = null,
        IEnumerable<int> teamIds = null,
        IEnumerable<int> playerIds = null)
        {
            var qs = new List<string> { $"per_page={perPage}", $"cursor={cursor}" };
            if (!string.IsNullOrWhiteSpace(search))
                qs.Add($"search={Uri.EscapeDataString(search)}");
            if (teamIds != null)
                qs.AddRange(teamIds.Select(id => $"team_ids[]={id}"));
            if (playerIds != null)
                qs.AddRange(playerIds.Select(id => $"player_ids[]={id}"));

            var url = "players?" + string.Join("&", qs);
            var resp = await _httpClientFactory.CreateClient("balldontlie").GetAsync(url);
            resp.EnsureSuccessStatusCode();

            var wrapper = await JsonSerializer.DeserializeAsync<ResponseWrapper<Player>>(
                await resp.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return (wrapper?.Data, wrapper?.Meta?.NextCursor);
        }

        public async Task<Player> GetPlayerByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("balldontlie");
            var response = await client.GetAsync($"players/{id}");
            response.EnsureSuccessStatusCode();

            var single = await JsonSerializer.DeserializeAsync<SingleResponseWrapper<Player>>(
                await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return single?.Data;
        }

        public async Task<(List<Player> players, int? nextCursor)> GetPlayersByTeamAsync(int teamId, int cursor = 0, int perPage = 25)
        {
            return await GetPlayersFilteredAsync(
                cursor, perPage,
                search: null,
                teamIds: new[] { teamId },
                playerIds: null);
        }
    }
}
