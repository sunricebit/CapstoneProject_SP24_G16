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
        public int MatchNumber { get; set; }
        [Required]
        public virtual Tournament tournament { get; set; }
        public int TableID { get; set; }
        public virtual Table table { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int TourID { get; set; }
        public int player1 { get; set; }
        public int player2 { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public bool IsFinish { get; set; }

    }
}
