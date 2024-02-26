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
        public Player? Players { get; set; }
        public int Player1 { get; set; }
        [Required]
        
        public int Player2 { get; set; }
        [Required]
        public int Score1 { get; set; }
        [Required]
        public int Score2 { get; set; }
        public DateTime StartTime { get; set; }
        public Type? Type { get; set; }
        public int TypeID { get; set; }
        public string Description { get; set; }
    }
}
