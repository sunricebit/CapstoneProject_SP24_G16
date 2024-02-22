using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Scale
    {
        [Key]
        public int ScaleID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Tournament> Tournament { get; set; }
    }
}
