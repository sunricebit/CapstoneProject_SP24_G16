using BusinessObject.Models;
using System.ComponentModel.DataAnnotations;

namespace PoolComVnWebAPI.DTO
{
        public class NewsDTO
        {
        
                public int NewsID { get; set; }
                public string Title { get; set; }
                public string? Description { get; set; }
                public int AccID { get; set; }
                public DateTime CreatedDate { get; set; }
                public DateTime UpdatedDate { get; set; }
                public string link { get; set; }
              
                public string? AccountName { get; set; }
        
        }
   
}
