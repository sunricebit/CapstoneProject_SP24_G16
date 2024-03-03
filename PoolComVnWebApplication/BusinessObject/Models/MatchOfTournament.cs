using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class MatchOfTournament
    {
        public MatchOfTournament()
        {
            PlayerInMatches = new HashSet<PlayerInMatch>();
        }

        public int MatchId { get; set; }
        public int TourId { get; set; }
        public int MatchNumber { get; set; }
        public string MatchCode { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public byte Status { get; set; }

        public virtual Tournament Tour { get; set; } = null!;
        public virtual ICollection<PlayerInMatch> PlayerInMatches { get; set; }
    }
}
