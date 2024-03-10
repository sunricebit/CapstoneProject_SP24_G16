﻿using BusinessObject.Models;
using System.ComponentModel.DataAnnotations;

namespace PoolComVnWebAPI.DTO
{
        public class NewsDTO
        {

        public int NewsId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int AccId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? Link { get; set; }
        public string? Flyer { get; set; }
        public string? AccountName { get; set; }
        
        }
   
}
