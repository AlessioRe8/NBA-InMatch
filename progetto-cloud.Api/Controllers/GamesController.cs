using Microsoft.AspNetCore.Mvc;
using progetto_cloud.Models;
using progetto_cloud.Services;

namespace progetto_cloud.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;
        public GamesController(IGameService svc) => _gameService = svc;

        // GET /api/games
        [HttpGet]
        public async Task<IEnumerable<Game>> GetAll()
        {
            var (games, _, _) = await _gameService.GetGamesAsync();
            return games;
        }

        // GET /api/games/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Game>> GetById(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null) return NotFound();
            return game;
        }

        // GET /api/games/filtered?seasons=2024&postseason=true&startDate=2025-01-01
        [HttpGet("filtered")]
        public async Task<IEnumerable<Game>> Filtered(
            [FromQuery] string? seasons,
            [FromQuery] string? teamIds,
            [FromQuery] bool? postseason,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var seasonList = string.IsNullOrWhiteSpace(seasons)
                ? null : seasons.Split(',').Select(int.Parse);
            var teamList = string.IsNullOrWhiteSpace(teamIds)
                ? null : teamIds.Split(',').Select(int.Parse);

            var (games, _, _) = await _gameService.GetGamesFilteredAsync(
                cursor: 0, perPage: 100,
                seasons: seasonList,
                teamIds: teamList,
                postseason: postseason,
                startDate: startDate,
                endDate: endDate);

            return games;
        }

        //GET /api/games/calendar/{season}
        [HttpGet("calendar/{season:int}")]
        public async Task<ActionResult<Dictionary<string, List<Game>>>> GetCalendar(int season, [FromQuery] int perPage = 100)
        {
            var allGames = await _gameService.GetAllGamesBySeasonDescendingAsync(season, perPage);

            var grouped = allGames
                .GroupBy(g => g.Date.Date)
                .OrderByDescending(g => g.Key)
                .ToDictionary(
                    grp => grp.Key.ToString("yyyy-MM-dd"),
                    grp => grp.ToList()
                );

            return Ok(grouped);
        }
    }
}