using progetto_cloud.Models;

namespace progetto_cloud.ViewModels
{
    public class GameFilterViewModel
    {
        public string Seasons { get; set; }
        public string TeamIds { get; set; }
        public bool? ShowPostseason { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<Game> Games { get; set; } = new();
        public int? NextCursor { get; set; }
        public int? PreviousCursor { get; set; }
        public int PageNumber { get; set; }
    }
}