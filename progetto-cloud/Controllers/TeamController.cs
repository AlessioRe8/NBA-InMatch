using Microsoft.AspNetCore.Mvc;
using progetto_cloud.Models.Common;
using progetto_cloud.Models;
using progetto_cloud.Services;
using System.Net.Http;
using System.Text.Json;
using System.ComponentModel;
using progetto_cloud.ViewModels;

namespace progetto_cloud.Controllers
{
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IPlayerService _playerService;
        private const int PageSize = 25;

        public TeamController(ITeamService teamService, IPlayerService playerService)
        {
            _teamService = teamService;
            _playerService = playerService;
        }

        public async Task<IActionResult> All(int cursor = 0, int pageNumber = 1)
        {
            var (games, nextCursor, prevCursor) = await _teamService.GetTeamsAsync(cursor, PageSize);

            ViewBag.CurrentCursor = cursor;
            ViewBag.NextCursor = nextCursor;
            ViewBag.PreviousCursor = prevCursor;
            ViewBag.PageNumber = pageNumber;

            return View(games);
        }

        public async Task<IActionResult> Filtered( int cursor = 0, int pageNumber = 1, string conference = null, string division = null)
        {
            var (teams, nextCursor, prevCursor) =
                await _teamService.GetTeamsFilteredAsync(cursor, PageSize, conference, division);

            var vm = new TeamFilterViewModel
            {
                Conference = conference,
                Division = division,
                Teams = teams,
                NextCursor = nextCursor,
                PreviousCursor = prevCursor,
                PageNumber = pageNumber
            };
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
                return NotFound();
            var (players, nextCursor) = await _playerService.GetPlayersByTeamAsync(id);

            var vm = new TeamDetailsViewModel
            {
                Team = team,
                Players = players,
                NextCursor = nextCursor
            };

            return View(vm);
        }

        public async Task<IActionResult> PlayersPartial(int teamId, int cursor = 0)
        {
            // recupera i prossimi 25 giocatori della squadra
            var (players, nextCursor) = await _playerService.GetPlayersByTeamAsync(teamId, cursor, 25);

            ViewBag.NextCursor = nextCursor;
            ViewBag.TeamId = teamId;

            return PartialView("_TeamPlayersPartial", players);
        }
    }
}
