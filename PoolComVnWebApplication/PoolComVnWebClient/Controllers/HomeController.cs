using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PoolComVnWebClient.Common;
using PoolComVnWebClient.DTO;
using PoolComVnWebClient.Models;
using System.Diagnostics;
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
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _logger = logger;
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
        public IActionResult NewsDetail()
        {
            return View();
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