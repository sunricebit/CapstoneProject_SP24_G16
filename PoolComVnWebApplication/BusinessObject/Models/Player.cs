﻿using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Player
    {
        public Player()
        {
            PlayerInMatches = new HashSet<PlayerInMatch>();
            PlayerInSoloMatches = new HashSet<PlayerInSoloMatch>();
        }

        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = null!;
        public int AccountId { get; set; }
        public int? CountryId { get; set; }
        public int Level { get; set; }
        public int UserId { get; set; }
        public int? TourId { get; set; }

        public virtual Country? Country { get; set; }
        public virtual Tournament? Tour { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual ICollection<PlayerInMatch> PlayerInMatches { get; set; }
        public virtual ICollection<PlayerInSoloMatch> PlayerInSoloMatches { get; set; }
    }
}
