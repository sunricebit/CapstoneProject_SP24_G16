using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Club
    {
        [Key]
        [Required]
        public int ClubId { get; set; }
        [Required]
        public string ClubName { get; set; }
        [Required]
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Facebook { get; set; }
        public string Avatar { get; set; }
        public List<ClubPost> ClubPost { get; set; }
    }
}
