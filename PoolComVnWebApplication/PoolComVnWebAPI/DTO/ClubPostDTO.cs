using BusinessObject.Models;

namespace PoolComVnWebAPI.DTO
{
    public class ClubPostDTO
    {
        public int PostID { get; set; }
        public int ClubID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? Image { get; set; }
    }
}
