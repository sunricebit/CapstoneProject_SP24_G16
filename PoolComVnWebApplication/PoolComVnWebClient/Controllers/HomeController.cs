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
        public IActionResult Index(int? page)
        {
            try
            {
                int pageNumber = page ?? 1;
                int pageSize = 6;
                var response = client.GetAsync($"{ApiUrl}/GetLatestNews").Result;
                response.EnsureSuccessStatusCode();
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var newsList = JsonConvert.DeserializeObject<List<NewsDTO>>(jsonContent);

                NewsDTO latestNews = null;
                foreach (var news in newsList)
                {
                    if (news.Status == true)
                    {
                        latestNews = news;
                        break;
                    }
                }
                var paginatedNewsList = PaginatedList<NewsDTO>.CreateAsync(newsList, pageNumber, pageSize);
                ViewBag.LatestNews = latestNews;
                return View(paginatedNewsList);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi kết nối đến API: " + ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi lấy danh sách tin tức: " + ex.Message);
                return View();
            }
        }
        [HttpGet]
        public IActionResult Manage()
        {
            string email = HttpContext.Request.Cookies["Email"];
            var response = client.GetAsync($"https://localhost:5000/api/Account/GetAccountByEmail/{email}").Result;
            var AccountData = response.Content.ReadAsStringAsync().Result;
            var account = JsonConvert.DeserializeObject<AccountDTO>(AccountData);
            if(account.RoleID == 2 || account.RoleID == 3)
            {
                return RedirectToAction("Index");
            }
            else if (account.RoleID == 1)
            {
                return RedirectToAction("Index", "Manager");
            }  
            else
            return RedirectToAction("Index", "NewsManage");
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

        [HttpGet]
        public IActionResult Club(int? page, string searchQuery)
        {
            try
            {
                int pageNumber = page ?? 1;
                int pageSize = 6;

                var searchClubsList = GetSearchClubsList(searchQuery);
                ViewBag.SearchQuery = searchQuery;

                var paginatedClubsList = PaginatedList<ClubDTO>.CreateAsync(searchClubsList, pageNumber, pageSize);
                return View(paginatedClubsList);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi kết nối đến API: " + ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi lấy danh sách Câu lạc bộ: " + ex.Message);
                return View();
            }
        }

        private List<ClubDTO> GetSearchClubsList(string searchQuery)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    var response = client.GetAsync($"https://localhost:5000/api/Club").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonContent = response.Content.ReadAsStringAsync().Result;
                        var clubsList = JsonConvert.DeserializeObject<List<ClubDTO>>(jsonContent);
                        return clubsList;
                    }
                    else
                    {
                        return new List<ClubDTO>();
                    }
                }
                else
                {
                    var response = client.GetAsync($"https://localhost:5000/api/Club/Search?searchQuery={searchQuery}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonContent = response.Content.ReadAsStringAsync().Result;
                        var clubsList = JsonConvert.DeserializeObject<List<ClubDTO>>(jsonContent);
                        return clubsList;
                    }
                    else
                    {
                        return new List<ClubDTO>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while getting search clubs list: {ex.Message}");
                return new List<ClubDTO>();
            }
        }

    }
}