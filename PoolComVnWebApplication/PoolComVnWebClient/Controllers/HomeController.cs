﻿using Microsoft.AspNetCore.Mvc;
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

                var visibleNewsList = newsList.Where(news => news.Status == true).ToList();

                while (visibleNewsList.Count < pageSize * pageNumber)
                {
                    visibleNewsList.AddRange(newsList.Where(news => news.Status == false).Take(pageSize * pageNumber - visibleNewsList.Count));
                }

                var paginatedNewsList = PaginatedList<NewsDTO>.CreateAsync(visibleNewsList, pageNumber, pageSize);
                NewsDTO latestNews = visibleNewsList.FirstOrDefault();

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
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
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
        public async Task<IActionResult> Club(int? page, string searchQuery, string provinceName, string? cityName)
        {
            try
            {
                int pageNumber = page ?? 1;
                int pageSize = 6;

                List<ClubDTO> clublists = null;
                if (!string.IsNullOrEmpty(provinceName) || !string.IsNullOrEmpty(cityName))
                {
                    clublists = await GetFilteredClubsByProvinceAndCity(provinceName, cityName);
                }
                else
                {
                    clublists = await GetSearchClubsList(searchQuery);
                }
                ViewBag.SearchQuery = searchQuery;
                if (clublists != null)
                {
                    var paginatedClubsList = PaginatedList<ClubDTO>.CreateAsync(clublists, pageNumber, pageSize);
                    return View(paginatedClubsList);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Lỗi khi lấy danh sách giải đấu từ API");
                    return RedirectToAction("InternalServerError", "Error");
                }
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

        private async Task<List<ClubDTO>?> GetFilteredClubsByProvinceAndCity(string provinceName, string? cityName)
        {
            try
            {
                string apiUrl = $"https://localhost:5000/api/Club/GetClubsByProvinceAndCity?provinceName={provinceName}";
                if (!string.IsNullOrEmpty(cityName) && cityName != "Quận/Huyện")
                {
                    apiUrl += $"&cityName={cityName}";
                }

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var clubsList = JsonConvert.DeserializeObject<List<ClubDTO>>(jsonContent);
                    return clubsList;
                }
                else
                {
                    return new List<ClubDTO>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while getting filtered clubs list: {ex.Message}");
                return new List<ClubDTO>();
            }
        }

        private async Task<List<ClubDTO>> GetSearchClubsList(string searchQuery)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    var response = await client.GetAsync($"https://localhost:5000/api/Club");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonContent = await response.Content.ReadAsStringAsync();
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
                    var response = await client.GetAsync($"https://localhost:5000/api/Club/Search?searchQuery={searchQuery}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonContent = await response.Content.ReadAsStringAsync();
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