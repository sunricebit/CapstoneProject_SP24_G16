using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class MatchOfTournament
    {
        [Key]
        public int TourMatchID { get; set; }
        [Required]
        public Tournament tournament { get; set; }

        public int TourID { get; set; }
        public int MatchNumber { get; set; }
        public Player player { get; set; }
        public int PlayerID { get; set; }
        public int Score { get; set; }

    }
}
