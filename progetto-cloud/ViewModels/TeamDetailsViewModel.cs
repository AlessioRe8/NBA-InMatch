using progetto_cloud.Models;

namespace progetto_cloud.ViewModels
{
    public class TeamDetailsViewModel
    {
        public Team Team { get; set; }
        public List<Player> Players { get; set; } = new();
        public int? NextCursor { get; set; }
    }
}
