using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Table
    {
        [Key] 
        public int TableId { get; set; }
        [Required]
        public virtual List<MatchOfTournament> MatchOfTournament { get; set; }
        public string TableName { get; set; }
    }
}
