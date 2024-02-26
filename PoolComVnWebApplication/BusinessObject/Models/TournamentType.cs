using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class TournamentType
    {
        [Key]
        public int TournamentTypeID { get; set; }
        [Required]
        public string TournamentTypeName { get; set; }
        public string? Description { get; set; }
        public virtual List<Tournament>? tournaments { get; set; }
    }
}
