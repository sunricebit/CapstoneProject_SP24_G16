﻿using System.ComponentModel.DataAnnotations;

namespace PoolComVnWebAPI.DTO
{
    public class ClubDTO
    {
        public int ClubId { get; set; }   
        public string ClubName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Facebook { get; set; }
        public string? Avatar { get; set; }
    }
}