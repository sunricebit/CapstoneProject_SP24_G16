using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using PoolComVnWebClient.DTO;
using PoolComVnWebClient.Common;

namespace PoolComVnWebClient.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly HttpClient client;
        private string ApiUrl = Constant.ApiUrl;

        public AuthenticationController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = ApiUrl + "/Authentication";
        }

        [HttpGet]
        public IActionResult Login(string? message)
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Tạo đối tượng chứa thông tin đăng nhập
            var loginInfo = new LoginDTO
            {
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
                    Expires = DateTime.UtcNow.AddDays(1),
                });

                Response.Cookies.Append("Email", email, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1),
                });

                return RedirectToAction("Index", "Home");
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                var responeGetId = await client.GetAsync(ApiUrl + "/GetIdByEmail?email=" + email);
                int responseId = await responeGetId.Content.ReadFromJsonAsync<int>();
                return await VerifyAccount(responseId, null);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                string errorMessage = "Email or password wrong!";
                return Login(errorMessage);
            }
            else
            {
                string errorMessage = "Account had been banned by admin.";
                return Login(errorMessage);
            }
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
                // Nhận accountId từ sever
                var responseData = await response.Content.ReadFromJsonAsync<int>();
                // Chuyển đến trang xác nhận đăng ký
                return RedirectToAction("VerifyAccount", "Authentication", new
                {
                    accountId = responseData,
                    message = string.Empty,
                });
            }
            else
            {
                var responseData = await response.Content.ReadFromJsonAsync<string>();
                // Xử lý khi đăng ký không thành công
                return RedirectToAction("Login", "Authentication", new { message = responseData });
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerifyAccount(int accountId, string? message)
        {
            ViewBag.AccountId = accountId;
            ViewBag.Message = message;
            var response = await client.GetAsync(ApiUrl + "/SendVerifyCode?accountId=" + accountId);
            return View("VerifyRegister");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyAccount(VerifyAccountDTO verifyAccountDTO)
        {
            var response = await client.PostAsJsonAsync(ApiUrl + "/VerifyAccount", verifyAccountDTO);

            // Kiểm tra xem yêu cầu có thành công hay không
            if (response.IsSuccessStatusCode)
            {
                // Nhận và giữ token từ phản hồi của server
                var responseMessage = await response.Content.ReadFromJsonAsync<VerifyDTO>();
                Response.Cookies.Append("TokenJwt", responseMessage.token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1),
                });

                Response.Cookies.Append("Email", responseMessage.Email, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1),
                });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                string message = "Verify Code wrong";
                return await VerifyAccount(verifyAccountDTO.AccountId, message);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("TokenJWT");
            Response.Cookies.Delete("Email");
            return RedirectToAction("Index", "Home");
        }
    }
}
