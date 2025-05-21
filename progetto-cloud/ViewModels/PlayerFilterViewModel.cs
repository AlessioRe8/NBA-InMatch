using progetto_cloud.Models;

namespace progetto_cloud.ViewModels
{
    public class PlayerFilterViewModel
    {
        public string Search { get; set; }
        public string TeamIds { get; set; }
        public string PlayerIds { get; set; }


        public List<Player> Players { get; set; } = new();
        public int? NextCursor { get; set; }
        public int PageNumber { get; set; }
        public int PreviousCursor { get; set; }
    }
}
