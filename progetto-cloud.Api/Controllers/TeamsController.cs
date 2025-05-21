using Microsoft.AspNetCore.Mvc;
using progetto_cloud.Services;
using progetto_cloud.Models;
using System.Text.Json.Serialization;

namespace progetto_cloud.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : Controller
    {
        private readonly ITeamService _teamService;
        public TeamsController(ITeamService svc) => _teamService = svc;

        // GET /api/teams
        [HttpGet]
        public async Task<IEnumerable<Team>> GetAll()
        {
            var (teams, nextCursor, prevCursor) = await _teamService.GetTeamsAsync();
            return teams;
        }

        // GET /api/teams/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Team>> GetById(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null) return NotFound();
            return team;
        }

        // GET /api/teams/filtered?conference=East&division=Atlantic
        [HttpGet("filtered")]
        public async Task<IEnumerable<Team>> Filtered(
            [FromQuery] Conference? conference,
            [FromQuery] Division? division)
        {
            var (teams, _, _) = await _teamService.GetTeamsFilteredAsync(
                cursor: 0,
                perPage: 100,
                conference: conference?.ToString(),
                division: division?.ToString());
            return teams;
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Conference
    {
        East,
        West
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Division
    {
        Atlantic,
        Central,
        Southeast,
        Northwest,
        Pacific,
        Southwest
    }
}

