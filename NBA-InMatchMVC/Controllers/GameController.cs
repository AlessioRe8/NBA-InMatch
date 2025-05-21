using Microsoft.AspNetCore.Mvc;
using progetto_cloud.Models;
using progetto_cloud.Services;
using progetto_cloud.ViewModels;
using System.Collections.Generic;

namespace progetto_cloud.Controllers
{
    public class GameController : Controller
    {

        private readonly IGameService _gameService;
        private const int PageSize = 25;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task<IActionResult> All(int cursor = 0, int pageNumber = 1)
        {
            var (games, nextCursor, prevCursor) = await _gameService.GetGamesAsync(cursor, PageSize);

            ViewBag.CurrentCursor = cursor;
            ViewBag.NextCursor = nextCursor;
            ViewBag.PreviousCursor = prevCursor;
            ViewBag.PageNumber = pageNumber;

            return View(games);
        }

        public async Task<IActionResult> Filtered(
            int cursor = 0,
            int pageNumber = 1,
            string seasons = null,
            string teamIds = null,
            bool? postseason = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Parse CSV in liste di int
            var seasonList = string.IsNullOrWhiteSpace(seasons)
                ? null
                : seasons.Split(',').Select(int.Parse);

            var teamList = string.IsNullOrWhiteSpace(teamIds)
                ? null
                : teamIds.Split(',').Select(int.Parse);

            var (games, nextCursor, prevCursor) = await _gameService.GetGamesFilteredAsync(
                cursor, PageSize,
                seasonList, teamList,
                postseason, startDate, endDate);

            var vm = new GameFilterViewModel
            {
                Seasons = seasons,
                TeamIds = teamIds,
                ShowPostseason = postseason,
                StartDate = startDate,
                EndDate = endDate,
                Games = games,
                NextCursor = nextCursor,
                PreviousCursor = prevCursor,
                PageNumber = pageNumber
            };
            return View(vm);
        }
        public async Task<IActionResult> Details(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null) return NotFound();
            return View(game);
        }
        public async Task<IActionResult> Calendar()
        {
            var all2024 = await _gameService.GetAllGamesBySeasonDescendingAsync(2024, perPage: 100);

            var grouped = all2024
                .GroupBy(g => g.Date.Date)
                .OrderByDescending(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());

            return View("Calendar", grouped);
        }
    }
}
