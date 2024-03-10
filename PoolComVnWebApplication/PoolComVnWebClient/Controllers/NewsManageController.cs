using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PoolComVnWebClient.Common;
using PoolComVnWebClient.DTO;
using System.Net;
using System.Net.Http.Headers;

namespace PoolComVnWebClient.Controllers
{
    public class NewsManageController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;

        public NewsManageController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = ApiUrl + "/News";
        }

        public IActionResult Index()
        {
            
            var response = client.GetAsync($"{ApiUrl}").Result;
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var newsList = JsonConvert.DeserializeObject<NewsDTO>(jsonContent);
                return View(newsList);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi lấy danh sách tin tức.");
                return View();
            }
        }

        public IActionResult AddNews()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddNews(NewsDTO newsDTO)
        {
            
            var response = client.PostAsJsonAsync($"{ApiUrl}", newsDTO).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi thêm tin tức.");
                return View(newsDTO);
            }
        }

        public IActionResult NewsDetails(int id)
        {
         
            var response = client.GetAsync($"{ApiUrl}/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var newsDetails = JsonConvert.DeserializeObject<NewsDTO>(jsonContent); 
                return View(newsDetails);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                
                return NotFound();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi lấy chi tiết tin tức.");
                return View();
            }
        }

        public IActionResult EditNews(int id)
        {
            
            var response = client.GetAsync($"{ApiUrl}/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var newsToEdit = JsonConvert.DeserializeObject<NewsDTO>(jsonContent); ;
                return View(newsToEdit);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                
                return NotFound();
            }
            else
            {
               
                ModelState.AddModelError(string.Empty, "Lỗi khi lấy tin tức để chỉnh sửa.");
                return View();
            }
        }

        [HttpPost]
        public IActionResult EditNews(int id, NewsDTO updatedNewsDTO)
        {
          
            var response = client.PutAsJsonAsync($"{ApiUrl}/{id}", updatedNewsDTO).Result;

            if (response.IsSuccessStatusCode)
            {
                
                return RedirectToAction("Index");
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                
                return NotFound();
            }
            else
            {
                
                ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật tin tức.");
                return View(updatedNewsDTO);
            }
        }

        [HttpPost]
        public IActionResult DeleteNews(int id)
        {
           
            var response = client.DeleteAsync($"{ApiUrl}/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                
                return RedirectToAction("Index");
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                
                return NotFound();
            }
            else
            {
               
                ModelState.AddModelError(string.Empty, "Lỗi khi xóa tin tức.");
                return View();
            }
        }
    }

}
