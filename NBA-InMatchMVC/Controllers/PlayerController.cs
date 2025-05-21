using Microsoft.AspNetCore.Mvc;
using progetto_cloud.Services;
using progetto_cloud.ViewModels;

namespace progetto_cloud.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;
        private const int PageSize = 25;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public async Task<IActionResult> All(int cursor = 0, int pageNumber = 1)
        {
            var (players, nextCursor, prevCursor) = await _playerService.GetPlayersAsync(cursor, PageSize);

            ViewBag.NextCursor = nextCursor;
            ViewBag.CurrentCursor = cursor;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PreviousCursor = prevCursor;

            return View(players);
        }

        public async Task<IActionResult> Filtered(int cursor = 0, int pageNumber = 1,
        string search = null,
        string teamIds = null,
        string playerIds = null)
        {
            var teamList = string.IsNullOrWhiteSpace(teamIds) ? null : teamIds.Split(',').Select(int.Parse);
            var playerList = string.IsNullOrWhiteSpace(playerIds) ? null : playerIds.Split(',').Select(int.Parse);

            var (players, nextCursor) = await _playerService.GetPlayersFilteredAsync(
                cursor, PageSize,
                search,
                teamList, playerList);

            var vm = new PlayerFilterViewModel
            {
                Search = search,
                TeamIds = teamIds,
                PlayerIds = playerIds,
                Players = players,
                NextCursor = nextCursor,
                PageNumber = pageNumber
            };
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null)
                return NotFound();

            return View(player);
        }
    }
}
