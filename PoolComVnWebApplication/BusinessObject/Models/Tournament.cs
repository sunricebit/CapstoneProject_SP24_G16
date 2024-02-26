using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Tournament
    {
        [Key]
        [Required]
        public int TourID { get; set; }
        [Required]
        public string TourName { get; set; }
        public string Description { get; set; }
        public int GameTypeID { get; set; }
        public int TournamentTypeID { get; set; }
        public int PlayerNumber { get; set; }
        public int EntryFee { get; set; }
        public int TotalPrice { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DrawType { get; set; }
        public string RaceToString { get; set; }
        public string PaymentType { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public string Rule { get;set; }
        public int AccessID { get; set; }
        public int MaxPlayerNumber { get; set; }

        public List<MatchOfTournament> MatchOfTournamentList { get; set; }
        public virtual Access access { get; set; }
        public virtual GameType type { get; set; }
        public virtual TournamentType tournamentType { get; set; }
        public virtual List<TourPlayer> TourPlayer { get; set; }





    }
}
