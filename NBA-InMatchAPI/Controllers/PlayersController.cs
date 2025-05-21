using Microsoft.AspNetCore.Mvc;
using progetto_cloud.Models;
using progetto_cloud.Services;

namespace progetto_cloud.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        public PlayersController(IPlayerService svc) => _playerService = svc;

        // GET /api/players
        [HttpGet]
        public async Task<IEnumerable<Player>> GetAll()
        {
            var (players, nextCursor, prevCursor) = await _playerService.GetPlayersAsync();
            return players;
        }

        // GET /api/players/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Player>> GetById(int id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null) return NotFound();
            return player;
        }

        // GET /api/players/filtered?search=smith&playerIds=1,2&teamIds=3,5
        [HttpGet("filtered")]
        public async Task<IEnumerable<Player>> Filtered(
            [FromQuery] string? search,
            [FromQuery] string? teamIds,
            [FromQuery] string? playerIds)
        {
            var tIds = string.IsNullOrWhiteSpace(teamIds)
                ? null : teamIds.Split(',').Select(int.Parse);
            var pIds = string.IsNullOrWhiteSpace(playerIds)
                ? null : playerIds.Split(',').Select(int.Parse);

            var (players, nextCursor) = await _playerService.GetPlayersFilteredAsync(
                cursor: 0, perPage: 100,
                search: search,
                teamIds: tIds,
                playerIds: pIds);
            return players;
        }
    }
}