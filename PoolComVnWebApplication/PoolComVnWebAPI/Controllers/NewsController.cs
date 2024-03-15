using BusinessObject.Models;
using DataAccess;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PoolComVnWebAPI.DTO;
using System.IO;
using System.Net.Sockets;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly NewsDAO _newsDAO;
        private static string ApiKey = "AIzaSyDbVNJE6bbQdXlcr3TZqxkZh3xqi5CqKIc";
        private static string Bucket = "poolcomvn-82664.appspot.com";
        private static string AuthEmail = "vuducduy@gmail.com";
        private static string AuthPassword = "123456";


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
                    NewsId = news.NewsId,
                    Title = news.Title,
                    Description = news.Description,
                    AccId = news.AccId,
                    CreatedDate = news.CreatedDate,
                    UpdatedDate = news.UpdatedDate,
                    Flyer = news.Flyer,
                    Link = news.Link,
                    AccountName = news.Acc?.Email
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest("Error while retrieving all news: " + ex.Message);
            }


        }

        [HttpGet("GetLatestNews")]
        public ActionResult<IEnumerable<NewsDTO>> GetLatestNews(int count)
        {
            try
            {
                var latestNews = _newsDAO.GetLatestNews(count);

                var result = latestNews.Select(news => new NewsDTO
                {
                    NewsId = news.NewsId,
                    Title = news.Title,
                    Description = news.Description,
                    AccId = news.AccId,
                    CreatedDate = news.CreatedDate,
                    UpdatedDate = news.UpdatedDate,
                    Flyer = news.Flyer,
                    Link = news.Link,
                    AccountName = news.Acc?.Email
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Error while retrieving latest news: " + ex.Message);
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
                    NewsId = news.NewsId,
                    Title = news.Title,
                    Description = news.Description,
                    AccId = news.AccId,
                    CreatedDate = news.CreatedDate,
                    UpdatedDate = news.UpdatedDate,
                    Link = news.Link,
                    Flyer = news.Flyer,
                    AccountName = news.Acc?.Email
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Post([FromForm] NewsDTO newsDTO, [FromForm] List<IFormFile> banner, [FromForm] List<IFormFile> images)
        {
            try
            {
                var account = _newsDAO.GetAccount(newsDTO.AccId);

                if (account == null)
                {

                    return BadRequest("Invalid AccID. No matching Account found.");
                }

                var news = new News
                {
                    Title = newsDTO.Title,
                    Description = newsDTO.Description,
                    AccId = newsDTO.AccId,
                    CreatedDate = newsDTO.CreatedDate,
                    UpdatedDate = newsDTO.UpdatedDate,
                    Link = newsDTO.Link,
                    Acc = account

                };

                if (banner != null)
                {
                    foreach (var ban in banner)
                    {
                        if (ban != null && ban.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ban.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);

                            using (FileStream memoryStream = new FileStream(filePath, FileMode.Create))
                            {
                                ban.CopyTo(memoryStream);


                            }
                            var fileStream2 = new FileStream(filePath, FileMode.Open);
                            var downloadLink = await UploadFromFirebase(fileStream2, ban.FileName, "News", newsDTO.Title, 0);
                            fileStream2.Close();
                            System.IO.File.Delete(filePath);

                        }


                    }
                }
                var order = 1;
                foreach (var image in images)
                {

                    if (image != null && image.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);
                        using (FileStream memoryStream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(memoryStream);



                        }
                        var fileStream2 = new FileStream(filePath, FileMode.Open);
                        var downloadLink = await UploadFromFirebase(fileStream2, image.FileName, "News", newsDTO.Title, order);
                        order++;
                        fileStream2.Close();
                        System.IO.File.Delete(filePath);
                    }
                }

                news.Flyer = $"https://console.firebase.google.com/project/poolcomvn-82664/storage/poolcomvn-82664.appspot.com/files/~2FNews~%2F{Uri.EscapeDataString(newsDTO.Title)}";
                _newsDAO.AddNews(news);
                return CreatedAtAction(nameof(Get), new { id = news.NewsId }, newsDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Update")]
        public ActionResult Put(int id, [FromBody] NewsDTO updatedNewsDTO)
        {
            try
            {
                if (id != updatedNewsDTO.NewsId)
                {
                    return BadRequest("Invalid News ID");
                }
                var account = _newsDAO.GetAccount(updatedNewsDTO.AccId);

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
                existingNews.AccId = updatedNewsDTO.AccId;
                existingNews.CreatedDate = updatedNewsDTO.CreatedDate;
                existingNews.UpdatedDate = updatedNewsDTO.UpdatedDate;
                existingNews.Link = updatedNewsDTO.Link;
                existingNews.Flyer = updatedNewsDTO.Flyer;
                existingNews.Acc = account;
                _newsDAO.UpdateNews(existingNews);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Delete")]
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

        private async Task<string> UploadFromFirebase(FileStream stream, string filename, string folderName, string newsTitle, int order)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
            var cancellation = new CancellationTokenSource();
            if (order == 0)
            {
                var task = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                ).Child(folderName)
                .Child(newsTitle)
                 .Child($"Banner")
                 .PutAsync(stream, cancellation.Token);
                try
                {
                    await task;
                    return filename;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception was thrown : {0}", ex);
                    return null;
                }
            }
            else
            {
                var orderedFileName = $"Image{order}{Path.GetExtension(stream.Name)}";
                var task = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                ).Child(folderName)
                .Child(newsTitle)
                 .Child(orderedFileName)
                 .PutAsync(stream, cancellation.Token);
                try
                {
                    await task;
                    return filename;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception was thrown : {0}", ex);
                    return null;
                }
            }

        }
        private async Task DeleteFromFirebase(string filename)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

                var cancellation = new CancellationTokenSource();
                var storage = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                );
                var oldImagesPath = $"News/{filename}";
                await storage.Child(oldImagesPath).DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred during deletion: {0}", ex);
            }
        }






    }
}
