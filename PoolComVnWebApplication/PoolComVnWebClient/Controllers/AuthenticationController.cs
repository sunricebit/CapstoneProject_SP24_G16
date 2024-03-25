using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using PoolComVnWebClient.DTO;
using PoolComVnWebClient.Common;
using Newtonsoft.Json;

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
        public IActionResult ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                // Kiểm tra ConfirmNewPassword có khớp với NewPassword hay không
                if (changePasswordDTO.NewPassword != changePasswordDTO.ConfirmNewPassword)
                {
                    TempData["ConfirmPasswordErrorMessage"] = "Mật khẩu mới và xác nhận mật khẩu mới không khớp.";
                    return RedirectToAction("ChangePassword");
                }
                string email = HttpContext.Request.Cookies["Email"];
                var response = client.GetAsync($"https://localhost:5000/api/Account/GetAccountByEmail/{email}").Result;
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Không thể lấy thông tin tài khoản.");
                    return View();
                }
                var AccountData = response.Content.ReadAsStringAsync().Result;
                var account = JsonConvert.DeserializeObject<AccountDTO>(AccountData);

                // So sánh mật khẩu cũ đã được mã hóa với mật khẩu từ cơ sở dữ liệu
                bool isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(changePasswordDTO.OldPassword, account.Password);
                if (!isOldPasswordCorrect)
                {
                    TempData["ErrorMessage"] = "Mật khẩu hiện tại không đúng.";
                    return View();
                }
                else
                {
                    // Nếu mật khẩu cũ đúng, gửi yêu cầu thay đổi mật khẩu
                    var response2 = client.PostAsJsonAsync(ApiUrl + "/ChangePassword", changePasswordDTO).Result;
                    if (response2.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Password changed successfully!";
                        return RedirectToAction("ChangePassword");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to change password.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View();
            }
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
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict
                });

                Response.Cookies.Append("Email", email, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict
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
        [HttpPost]
        public async Task<IActionResult> SendEmailContact(ContactDTO contactDTO)
        {
            try
            {
                var response = await client.PostAsJsonAsync(ApiUrl + "/SendEmailContact", contactDTO);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Email sent successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to send email.";
                }
                return RedirectToAction("Contact", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Contact", "Home");
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
