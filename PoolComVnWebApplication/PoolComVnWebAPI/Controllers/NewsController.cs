﻿using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PoolComVnWebAPI.DTO;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly NewsDAO _newsDAO;

        public NewsController(NewsDAO newsDAO)
        {
            _newsDAO = newsDAO;
        }

        [HttpGet]
        public ActionResult<IEnumerable<NewsDTO>> Get()
        {
            try
            {
                
                    var allNews = _newsDAO.GetAllNews();

                var result = allNews.Select(news => new NewsDTO
                {
                    NewsID = news.NewsId,
                    Title = news.Title,
                    Description = news.Description,
                    AccID = news.AccId,
                    CreatedDate = news.CreatedDate,
                    UpdatedDate = news.UpdatedDate,
                    link = news.Link,
                    AccountName = news.Acc?.Email
                }).ToList();

                return Ok(result);
                }
                catch (Exception ex)
                {
                    
                    return BadRequest("Error while retrieving all news: " + ex.Message);
                }
            
            
        }

        
        [HttpGet("{id}")]
        public ActionResult<NewsDTO> Get(int id)
        {
            try
            {
                var news = _newsDAO.GetNewsById(id);

                if (news == null)
                {
                    return NotFound(); 
                }
                var result = new NewsDTO
                {
                    NewsID = news.NewsId,
                    Title = news.Title,
                    Description = news.Description,
                    AccID = news.AccId,
                    CreatedDate = news.CreatedDate,
                    UpdatedDate = news.UpdatedDate,
                    link = news.Link,
                    AccountName = news.Acc?.Email
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

     
        [HttpPost]
        public ActionResult Post([FromBody] NewsDTO newsDTO)
        {
            try
            {
                var account =_newsDAO.GetAccount(newsDTO.AccID);

                if (account == null)
                {
                    
                    return BadRequest("Invalid AccID. No matching Account found.");
                }

                var news = new News
                {
                    Title = newsDTO.Title,
                    Description = newsDTO.Description,
                    AccId = newsDTO.AccID,
                    CreatedDate = newsDTO.CreatedDate,
                    UpdatedDate = newsDTO.UpdatedDate,
                    Link = newsDTO.link,
                    Acc = account
                    
                };

                _newsDAO.AddNews(news);
                return CreatedAtAction(nameof(Get), new { id = news.NewsId }, newsDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

      
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] NewsDTO updatedNewsDTO)
        {
            try
            {
                if (id != updatedNewsDTO.NewsID)
                {
                    return BadRequest("Invalid News ID"); 
                }
                var account = _newsDAO.GetAccount(updatedNewsDTO.AccID);

                if (account == null)
                {

                    return BadRequest("Invalid AccID. No matching Account found.");
                }



                var existingNews = _newsDAO.GetNewsById(id);

                if (existingNews == null)
                {
                    return NotFound(); 
                }

                existingNews.Title = updatedNewsDTO.Title;
                existingNews.Description = updatedNewsDTO.Description;
                existingNews.AccId = updatedNewsDTO.AccID;
                existingNews.CreatedDate = updatedNewsDTO.CreatedDate;
                existingNews.UpdatedDate = updatedNewsDTO.UpdatedDate;
                existingNews.Link = updatedNewsDTO.link;
                existingNews.Acc = account; 
                _newsDAO.UpdateNews(existingNews);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id, [FromBody] NewsDTO deletedNewsDTO)
        {
            try
            {
                if (id != deletedNewsDTO.NewsID)
                {
                    return BadRequest("Invalid News ID");
                }

                _newsDAO.DeleteNews(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
