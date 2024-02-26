using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Player
    {
        [Key]
        [Required]
        public int PlayerID { get; set; }
        public string PlayerName { get; set; }
        public Account Account { get; set; }
        public int AccountID { get; set; }
        public string Level { get; set; }
        public List<TourPlayer> TourPlayer { get; set; }
        public MatchOfTournament MatchOfTournament { get; set;}
        public List<SoloMatch> soloMatches { get; set; }

    }
}
