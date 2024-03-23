using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PoolComVnWebClient.Common;
using PoolComVnWebClient.DTO;
using System.Net;
using System.Net.Http.Headers;

namespace PoolComVnWebClient.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;
        private string ApiKey = FirebaseAPI.ApiKey;
        private string Bucket = FirebaseAPI.Bucket;
        private string AuthEmail = FirebaseAPI.AuthEmail;
        private string AuthPassword = FirebaseAPI.AuthPassword;
        public UserController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }
        public IActionResult Index()
        {
            string email = HttpContext.Request.Cookies["Email"];
            var response = client.GetAsync($"{ApiUrl}/Account/GetAccountByEmail/{email}").Result;
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Không thể lấy thông tin tài khoản.");
                return View();
            }
            var AccountData = response.Content.ReadAsStringAsync().Result;
            var account = JsonConvert.DeserializeObject<AccountDTO>(AccountData);
            var responeUser = client.GetAsync($"{ApiUrl}/Account/GetUserByAccount/{account.AccountID}").Result;
            if (responeUser.StatusCode == HttpStatusCode.NotFound)
            {
                return RedirectToAction("CreateUser");
            }
            else if (responeUser.IsSuccessStatusCode)
            {
                var UserData = responeUser.Content.ReadAsStringAsync().Result;
                var user = JsonConvert.DeserializeObject<UserDTO>(UserData);
                ViewBag.User = user;
            }
            return View();
        }
        public IActionResult CreateUser()
        {
            return View();
        }

        public IActionResult EditUserProfile()
        {
            return View();
        }
    }
}
