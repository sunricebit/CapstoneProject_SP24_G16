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
        public string Fullname { get; set; }
        [Required]
        public string Nickname { get; set; }
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public int RoleID { get; set; }
        public string veriyCode { get; set; }   
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<Order> OrderList { get; set; }
        public List<News> NewsList { get; set; }
        public Player Player { get; set; }
        public Club Club { get; set; }

    }
}
