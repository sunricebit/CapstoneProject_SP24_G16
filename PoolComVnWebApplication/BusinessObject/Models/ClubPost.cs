using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ClubPost
    {
        [Key]
        public int PostID { get; set; }
        public int ClubID { get; set; }
        public Club Club { get; set; }
        public string ClubName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Facebook { get; set; }
        public string Avatar { get; set; }
    }
}
