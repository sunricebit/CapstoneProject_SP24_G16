using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class User
    {
        public User()
        {
            Players = new HashSet<Player>();
        }

        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime Dob { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Player UserNavigation { get; set; } = null!;
        public virtual ICollection<Player> Players { get; set; }
    }
}
