using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class User
    {
        [Key] public int UserId { get; set; }
        [Required]
         public string FullName { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set;}
        public virtual Account Account { get; set; }
        public virtual Player Player { get; set; }

    }
}
