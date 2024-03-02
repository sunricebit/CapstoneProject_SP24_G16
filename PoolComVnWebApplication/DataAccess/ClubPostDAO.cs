using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ClubPostDAO
    {
        private readonly PoolComContext _context;

        public ClubPostDAO(PoolComContext context)
        {
            _context = context;
        }

        // Create ClubPost
        public void AddClubPost(ClubPost clubPost)
        {
            _context.ClubPosts.Add(clubPost);
            _context.SaveChanges();
        }

        // Read ClubPost
        public ClubPost GetClubPostById(int postId)
        {
            return _context.ClubPosts.Find(postId);
        }

        public List<ClubPost> GetAllClubPosts()
        {
            return _context.ClubPosts.ToList();
        }

        // Update ClubPost
        public void UpdateClubPost(ClubPost updatedClubPost)
        {
            var existingClubPost = _context.ClubPosts.Find(updatedClubPost.PostID);

            if (existingClubPost != null)
            {
                existingClubPost.Title = updatedClubPost.Title;
                existingClubPost.Description = updatedClubPost.Description;
                existingClubPost.CreatedDate = updatedClubPost.CreatedDate;
                existingClubPost.UpdatedDate = updatedClubPost.UpdatedDate;
                existingClubPost.Image = updatedClubPost.Image;

                _context.SaveChanges();
            }
        }

        // Delete ClubPost
        public void DeleteClubPost(int postId)
        {
            var postToDelete = _context.ClubPosts.Find(postId);

            if (postToDelete != null)
            {
                _context.ClubPosts.Remove(postToDelete);
                _context.SaveChanges();
            }
        }
    }
}

