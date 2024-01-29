using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Role
    {
        [Key]
        [Required]
        public int RoleID { get; set; }
        [Required]
        public string RoleName { get; set; }
        public List<Account> Account { get; set; }

    }
}
