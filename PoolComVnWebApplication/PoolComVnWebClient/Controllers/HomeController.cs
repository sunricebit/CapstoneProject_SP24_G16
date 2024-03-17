using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PoolComVnWebClient.Common;
using PoolComVnWebClient.DTO;
using PoolComVnWebClient.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;

namespace PoolComVnWebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = ApiUrl + "/News";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await client.GetAsync($"{ApiUrl}/GetLatestNews?count=6"); // Lấy 6 tin tức mới nhất
                response.EnsureSuccessStatusCode(); // Đảm bảo phản hồi thành công

                var jsonContent = await response.Content.ReadAsStringAsync();
                var newsList = JsonConvert.DeserializeObject<List<NewsDTO>>(jsonContent);
                return View(newsList);
            }
            catch (HttpRequestException ex)
            {
                // Xử lý lỗi nếu không thể kết nối đến API
                ModelState.AddModelError(string.Empty, "Lỗi khi kết nối đến API: " + ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                ModelState.AddModelError(string.Empty, "Lỗi khi lấy danh sách tin tức: " + ex.Message);
                return View();
            }
        }


        [HttpGet]
        public IActionResult NewsDetail(int id)
        {
            var response = client.GetAsync($"{ApiUrl}/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var newsDetails = JsonConvert.DeserializeObject<NewsDTO>(jsonContent);

                var response2 = client.GetAsync($"{ApiUrl}/GetLatestNews?count=6").Result;
                response2.EnsureSuccessStatusCode();

                var jsonContent2 = response2.Content.ReadAsStringAsync().Result;
                var newsList = JsonConvert.DeserializeObject<List<NewsDTO>>(jsonContent2);
                ViewBag.OtherNewsList = newsList;
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

        [HttpGet]
        public IActionResult GameRules()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}