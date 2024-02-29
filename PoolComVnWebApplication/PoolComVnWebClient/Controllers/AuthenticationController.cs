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
                var responseData = await response.Content.ReadFromJsonAsync<string>();
                //var token = responseData.token;

                // Lưu token vào nơi phù hợp, có thể lưu vào Session, Cache, hoặc nơi khác
                // để sử dụng trong các yêu cầu sau này
            }
            else
            {
                // Xử lý khi đăng nhập không thành công
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
