using progetto_cloud.Models;

namespace progetto_cloud.ViewModels
{
    public class TeamFilterViewModel
    {
        public string Conference { get; set; }
        public string Division { get; set; }


        public List<Team> Teams { get; set; } = new();
        public int? NextCursor { get; set; }
        public int? PreviousCursor { get; set; }
        public int PageNumber { get; set; }
    }
}
