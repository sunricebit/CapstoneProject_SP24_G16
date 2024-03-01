using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using PoolComVnWebClient.DTO;
using PoolComVnWebAPI.DTO;
using PoolComVnWebClient.Common;
using System.Reflection.Metadata.Ecma335;

namespace PoolComVnWebClient.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrl = "";

        public AuthenticationController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = "https://localhost:7123/api/Authentication";
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // HttpResponseMessage responseMessage = await client.GetAsync(ApiUrl + "/login");

            // Tạo đối tượng chứa thông tin đăng nhập
            var loginInfo = new LoginDTO { 
                Email = email, 
                Password = password
            };

            // Gửi yêu cầu POST đến endpoint đăng nhập
            var response = await client.PostAsJsonAsync(ApiUrl + "/login", loginInfo);

            // Kiểm tra xem yêu cầu có thành công hay không
            if (response.IsSuccessStatusCode)
            {
                // Nhận và giữ token từ phản hồi của server
                var responseMessage = await response.Content.ReadFromJsonAsync<TokenJWT>();
                Response.Cookies.Append("TokenJwt", responseMessage.token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1),
                });
            }
            else
            {
                
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string rName, string rEmail, string rPassword, string AccountRole)
        {
            var registerInfo = new RegisterDTO
            {
                Email = rEmail,
                Pass = rPassword,
                Username = rName,
                isBusiness = AccountRole == Constant.StrBusinessRole ? true : false,
            };

            var response = await client.PostAsJsonAsync(ApiUrl + "/register", registerInfo);

            // Kiểm tra xem yêu cầu có thành công hay không
            if (response.IsSuccessStatusCode)
            {
                // Nhận và giữ token từ phản hồi của server
                var responseData = await response.Content.ReadFromJsonAsync<string>();

            }
            else
            {
                // Xử lý khi đăng ký không thành công
            }
            return RedirectToAction("Login", "Authentication");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyRegister()
        {
            return View();
        }
    }
}
