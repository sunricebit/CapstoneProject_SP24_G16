using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolComVnWebAPI.DTO
{
    public class PlayerDTO
    {

        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = null!;
        public int? CountryId { get; set; }
        public int? Level { get; set; }
        public int? UserId { get; set; }
        public int? TourId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool? IsPayed { get; set; }
        public string CountryName { get; set; }
    }
}
