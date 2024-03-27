using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PoolComVnWebClient.Common;
using PoolComVnWebClient.DTO;
using System.Net.Http.Headers;

namespace PoolComVnWebClient.Controllers
{
    public class ManagerController : Controller
    {

        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;
        public ManagerController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }
        public IActionResult Index()
        {
            var responseManageAccount = client.GetAsync($"{ApiUrl}"+ "/Account/GetManagerAccounts").Result;
            var responseAccount = client.GetAsync($"{ApiUrl}" + "/Account").Result;
            var responseClub = client.GetAsync($"{ApiUrl}" + "/Club").Result;
            var responseUser = client.GetAsync($"{ApiUrl}" + "/User/GetAllUser").Result;
            if (responseManageAccount.IsSuccessStatusCode && responseClub.IsSuccessStatusCode 
                && responseAccount.IsSuccessStatusCode && responseUser.IsSuccessStatusCode)
            {
                var viewModel = new ManagerDTO();

                //Lấy danh sách Account để hiển thị và lấy các trường liên quan cho club và user
                var jsonContentAccount = responseAccount.Content.ReadAsStringAsync().Result;
                var account = JsonConvert.DeserializeObject<IEnumerable<AccountDTO>>(jsonContentAccount);

                //Lấy danh sách BusinessManageAccount để hiển thị
                var jsonContentManageAccount = responseManageAccount.Content.ReadAsStringAsync().Result;
                viewModel.Accounts = JsonConvert.DeserializeObject<IEnumerable<AccountDTO>>(jsonContentManageAccount);

                //Lấy danh sách Club để hiển thị
                var jsonContentClub = responseClub.Content.ReadAsStringAsync().Result;
                viewModel.Clubs = JsonConvert.DeserializeObject<IEnumerable<ClubDTO>>(jsonContentClub);
                foreach (var club in viewModel.Clubs)
                {
                    var ownerAccount = account.FirstOrDefault(a => a.AccountID == club.AccountId);
                    if (ownerAccount != null)
                    {
                        club.AccountEmail = ownerAccount.Email;
                    }
                }

                //Lấy danh sách User để hiển thị
                var jsonContentUser = responseUser.Content.ReadAsStringAsync().Result;
                viewModel.Users = JsonConvert.DeserializeObject<IEnumerable<UserDTO>>(jsonContentUser);
                foreach (var user in viewModel.Users)
                {
                    var ownerAccount = account.FirstOrDefault(a => a.AccountID == user.AccountId);
                    if (ownerAccount != null)
                    {
                        user.AccountEmail = ownerAccount.Email;
                        user.PhoneNumber = ownerAccount.PhoneNumber;
                        user.Status = ownerAccount.Status;
                    }
                }
                return View(viewModel);
            }
            else
            {
                return View("Error");
            }

        }

        public IActionResult CreateManageAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateManageAccount(CreateManageAccountDTO businessDTO)
        {
            var responseAccount = await client.GetAsync($"{ApiUrl}" + "/Account");
            if (!responseAccount.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Không thể lấy thông tin tài khoản.");
                return View();
            }
            var accountList = JsonConvert.DeserializeObject<List<AccountDTO>>(await responseAccount.Content.ReadAsStringAsync());

            // Kiểm tra xem email đã tồn tại trong danh sách account hay chưa
            if (accountList.Any(a => a.Email == businessDTO.Email))
            {
                TempData["ErrorMessage"] = "Tài khoản đã tồn tại.";
                return RedirectToAction("CreateManageAccount");
            }

            if (businessDTO.NewPassword != businessDTO.ConfirmNewPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới và xác nhận mật khẩu mới không khớp.";
                return RedirectToAction("CreateManageAccount");
            }
            try
            {
                var response = await client.PostAsJsonAsync(ApiUrl + "/Account/CreateBusinessManagerAccount", businessDTO);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Add new business manager successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add new business manager.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View();
            }
        }



    }
}
