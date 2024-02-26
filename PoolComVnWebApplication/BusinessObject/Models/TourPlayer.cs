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
        public Tournament Tournament { get; set; }
        public int TourID { get; set; }
        [Required]
        public Player Player { get; set; }
        public int PlayerID { get; set; }
    }
}
