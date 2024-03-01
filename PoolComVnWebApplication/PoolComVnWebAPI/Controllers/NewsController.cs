using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<IEnumerable<News>> Get()
        {
            var allNews = _newsDAO.GetAllNews();
            return Ok(allNews);
        }

        // GET: api/News/5
        [HttpGet("{id}")]
        public ActionResult<News> Get(int id)
        {
            try
            {
                var news = _newsDAO.GetNewsById(id);

                if (news == null)
                {
                    return NotFound(); 
                }

                return Ok(news);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

     
        [HttpPost]
        public ActionResult Post([FromBody] News news)
        {
            try
            {
                _newsDAO.AddNews(news);
                return CreatedAtAction(nameof(Get), new { id = news.NewsID }, news);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

      
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] News updatedNews)
        {
            try
            {
                if (id != updatedNews.NewsID)
                {
                    return BadRequest("Invalid News ID"); 
                }

                _newsDAO.UpdateNews(updatedNews);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
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
