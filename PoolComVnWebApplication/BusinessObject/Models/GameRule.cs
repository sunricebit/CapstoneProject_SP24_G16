using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class GameRule
    {
        [Key]
        [Required]
        public int GameRuleID { get; set; }
        [Required]
        public string RuleName { get; set; }
        public string Description { get; set; }
    }
}
