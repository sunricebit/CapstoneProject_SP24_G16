using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class PostDAO
    {
        private readonly PoolComContext _context;
        public PostDAO(PoolComContext context)
        {
            _context = context;
        }
        public void AddNews(News news)
        {
            if (news == null)
            {
                throw new ArgumentNullException(nameof(news));
            }

            if (string.IsNullOrWhiteSpace(news.Title))
            {
                throw new ArgumentException("News title cannot be null or empty", nameof(news.Title));
            }

            if (string.IsNullOrWhiteSpace(news.Description))
            {
                throw new ArgumentException("News description cannot be null or empty", nameof(news.Description));
            }

            _context.News.Add(news);
            _context.SaveChanges();
        }
        public News GetNewsById(int newsId)
        {
            try
            {
                return _context.News
                    .Include(n => n.Account)
                    .FirstOrDefault(n => n.NewsID == newsId);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần thiết
                throw new Exception($"Error while retrieving news with ID {newsId}.", ex);
            }
        }
        public List<News> GetAllNews()
        {
            try
            {
                return _context.News
                    .Include(n => n.Account)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần thiết
                throw new Exception("Error while retrieving all news.", ex);
            }
        }
        public void UpdateNews(News updatedNews)
        {
            if (updatedNews == null)
            {
                throw new ArgumentNullException(nameof(updatedNews));
            }

            if (string.IsNullOrWhiteSpace(updatedNews.Title))
            {
                throw new ArgumentException("Updated news title cannot be null or empty", nameof(updatedNews.Title));
            }

            if (string.IsNullOrWhiteSpace(updatedNews.Description))
            {
                throw new ArgumentException("Updated news description cannot be null or empty", nameof(updatedNews.Description));
            }

            var existingNews = _context.News.Find(updatedNews.NewsID);

            if (existingNews != null)
            {
                existingNews.Title = updatedNews.Title;
                existingNews.Description = updatedNews.Description;
                existingNews.UpdatedDate = DateTime.Now;

                _context.SaveChanges();
            }
        }
        public void DeleteNews(int newsId)
        {
            if (newsId <= 0)
            {
                throw new ArgumentException("News ID must be greater than 0", nameof(newsId));
            }

            var newsToDelete = _context.News.Find(newsId);

            if (newsToDelete != null)
            {
                _context.News.Remove(newsToDelete);
                _context.SaveChanges();
            }
        }

    }
}
