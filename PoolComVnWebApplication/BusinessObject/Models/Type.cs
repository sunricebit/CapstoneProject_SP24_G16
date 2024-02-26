using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Type
    {
        [Key]
        [Required]
        public int TypeID { get; set; }
        [Required]
        public string TypeName { get; set; }
        public string Description { get; set; }
        public List<SoloMatch> Match { get; set; }
        public List<Tournament> Tournament { get; set; }
    }
}
