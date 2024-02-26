using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Access
    {
        [Key]
        public int AccessID { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public virtual List<Tournament>? tournaments { get; set; }
    }
}
