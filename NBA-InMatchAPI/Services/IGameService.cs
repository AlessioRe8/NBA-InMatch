using Microsoft.Extensions.Caching.Memory;
using progetto_cloud.Models;
using progetto_cloud.Models.Common;
using System.Net.Http;
using System.Text.Json;

namespace progetto_cloud.Services
{
    public interface IGameService
    {
        Task<(List<Game> games, int? nextCursor, int? prevCursor)> GetGamesAsync(int cursor = 0, int perPage = 25);
        Task<(List<Game> games, int? nextCursor, int? prevCursor)> GetGamesFilteredAsync(
        int cursor = 0,
        int perPage = 25,
        IEnumerable<int> seasons = null,
        IEnumerable<int> teamIds = null,
        bool? postseason = null,
        DateTime? startDate = null,
        DateTime? endDate = null);

        Task<Game> GetGameByIdAsync(int id);
        Task<List<Game>> GetAllGamesBySeasonDescendingAsync(int season, int perPage = 100);

    }
    public class GameService : IGameService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public GameService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        

        public async Task<(List<Game> games, int? nextCursor, int? prevCursor)> GetGamesAsync(int cursor = 0, int perPage = 25)
        {
            var client = _httpClientFactory.CreateClient("balldontlie");

            var url = $"games?per_page={perPage}&cursor={cursor}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var wrapper = await JsonSerializer.DeserializeAsync<ResponseWrapper<Game>>(
                await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return (
                wrapper?.Data ?? new List<Game>(),
                wrapper?.Meta?.NextCursor,
                wrapper?.Meta?.PreviousCursor
            );
        }

        public async Task<(List<Game> games, int? nextCursor, int? prevCursor)> GetGamesFilteredAsync(
        int cursor = 0,
        int perPage = 25,
        IEnumerable<int> seasons = null,
        IEnumerable<int> teamIds = null,
        bool? postseason = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
        {
            var client = _httpClientFactory.CreateClient("balldontlie");
            var qs = new List<string>
        {
            $"per_page={perPage}",
            $"cursor={cursor}"
        };

            if (seasons != null)
                qs.AddRange(seasons.Select(s => $"seasons[]={s}"));
            if (teamIds != null)
                qs.AddRange(teamIds.Select(id => $"team_ids[]={id}"));
            if (postseason.HasValue)
                qs.Add($"postseason={postseason.Value.ToString().ToLower()}");
            if (startDate.HasValue)
                qs.Add($"start_date={startDate:yyyy-MM-dd}");
            if (endDate.HasValue)
                qs.Add($"end_date={endDate:yyyy-MM-dd}");

            var url = "games?" + string.Join("&", qs);
            var resp = await client.GetAsync(url);
            resp.EnsureSuccessStatusCode();

            var wrapper = await JsonSerializer.DeserializeAsync<ResponseWrapper<Game>>(
                await resp.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return (
                wrapper?.Data ?? new List<Game>(),
                wrapper?.Meta?.NextCursor,
                wrapper?.Meta?.PreviousCursor
            );
        }
        public async Task<Game> GetGameByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("balldontlie");
            var resp = await client.GetAsync($"games/{id}");
            resp.EnsureSuccessStatusCode();

            var single = await JsonSerializer.DeserializeAsync<SingleResponseWrapper<Game>>(
                await resp.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return single?.Data;
        }

        public async Task<List<Game>> GetAllGamesBySeasonDescendingAsync(int season, int perPage = 100)
        {

            var allGames = new List<Game>();
            int? cursor = 0;

            do
            {
                // Riusa il metodo di filtraggio che supporta seasons[]
                var (batch, nextCursor, _) = await GetGamesFilteredAsync(
                    cursor: cursor ?? 0,
                    perPage: perPage,
                    seasons: new[] { season },
                    teamIds: null,
                    postseason: null,
                    startDate: null,
                    endDate: null);

                allGames.AddRange(batch);
                cursor = nextCursor;
            }
            while (cursor != null);

            var result = allGames.OrderByDescending(g => g.Date).ToList();

            return result;
        }

    }
}
