using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class GameType
    {
        [Key]
        [Required]
        public int GameTypeID { get; set; }
        [Required]
        public string TypeName { get; set; }
        public string? Description { get; set; }
        public virtual List<SoloMatch> Match { get; set; }
        public virtual List<Tournament> Tournament { get; set; }
    }
}
