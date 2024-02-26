using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class TourPlayer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PlayerID { get; set; }
        [Required]
        public int TournamentID { get; set; }
        public virtual Player Player { get; set; }
        public virtual Tournament Tournament { get; set; }

    }
}
