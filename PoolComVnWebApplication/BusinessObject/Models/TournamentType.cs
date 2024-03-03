using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class TournamentType
    {
        public TournamentType()
        {
            TournamentTournamentTypes = new HashSet<Tournament>();
        }

        public int TournamentTypeId { get; set; }
        public string TournamentTypeName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual Tournament? TournamentTour { get; set; }
        public virtual ICollection<Tournament> TournamentTournamentTypes { get; set; }
    }
}
