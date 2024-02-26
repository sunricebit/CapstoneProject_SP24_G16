using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Account
    {
        [Key]
        [Required]
        public int AccountID { get; set; }
        [Required]
  
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual Role Role { get; set; }
        public int RoleID { get; set; }
        public string? PhoneNumber { get; set; }
        public string? verifyCode { get; set; }
        
        public virtual List<News>? NewsList { get; set; }
        public virtual Player? Player { get; set; }
        public virtual Club? Club { get; set; }
        public bool Status { get; set; }

    }
}
