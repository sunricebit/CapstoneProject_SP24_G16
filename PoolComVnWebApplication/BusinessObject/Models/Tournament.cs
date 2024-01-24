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
        public int TournamentID { get; set; }
        [Required]
        public string TourName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int GameRuleID { get; set; }
        public int ScaleID { get; set; }
        public int MinPlayer { get; set; }
        public int MaxPlayer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }



    }
}
