using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using PoolComVnWebClient.Common;
using Newtonsoft.Json;
using PoolComVnWebClient.DTO;
using System.Net;

namespace PoolComVnWebClient.Controllers
{
    public class ClubController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;
        public ClubController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }
        public IActionResult Index(string email)
        {
            var response = client.GetAsync($"{ApiUrl}/Account/GetAccountByEmail/{email}").Result;
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Không thể lấy thông tin tài khoản.");
                return View();
            }
            var AccountData = response.Content.ReadAsStringAsync().Result;
            var account = JsonConvert.DeserializeObject<AccountDTO>(AccountData);
            var response2 = client.GetAsync($"{ApiUrl}/Club/GetClubByAccountId/?accountID={account.AccountID}").Result;
            if (!response.IsSuccessStatusCode)
            {
                if (response2.StatusCode == HttpStatusCode.NotFound)
                {
                    return View();
                }
                else
                {
                    
                    ModelState.AddModelError(string.Empty, "Không thể lấy thông tin câu lạc bộ.");
                    return View();
                }
            }
            var ClubData = response2.Content.ReadAsStringAsync().Result;
            var club = JsonConvert.DeserializeObject<ClubDTO>(ClubData);
            ViewBag.AccountEmail = email;
            return View(club);
        }

        public IActionResult ClubTournament()
        {
            return View();
        }

        public IActionResult ClubSoloMatch()
        {
            return View();
        }

        public IActionResult ClubSoloMatchDetail()
        {
            return View();
        }
    }
}
