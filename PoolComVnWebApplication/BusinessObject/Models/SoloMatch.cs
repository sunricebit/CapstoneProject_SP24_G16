using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class SoloMatch
    {
        [Key]
        public int SoloMatchID { get; set; }
        [Required]
        public int Player1 { get; set; }
        [Required]
        
        public int Player2 { get; set; }
        [Required]
        public int Score1 { get; set; }
        [Required]
        public int Score2 { get; set; }
        public DateTime StartTime { get; set; }
        public virtual GameType? Type { get; set; }
        public int GameTypeID { get; set; }
        public int ClubID { get; set; }
        public virtual Club Club { get; set; }
        public string Description { get; set; }
    }
}
