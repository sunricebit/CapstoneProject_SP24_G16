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
        public IActionResult Index(int? page, string searchQuery)
        {
            var viewModel = new ManagerDTO();
            var responseManageAccount = client.GetAsync($"{ApiUrl}"+ "/Account/GetManagerAccounts").Result;
            var responseAccount = client.GetAsync($"{ApiUrl}" + "/Account").Result;
            var responseClub = client.GetAsync($"{ApiUrl}" + "/Club").Result;
            var responseUser = client.GetAsync($"{ApiUrl}" + "/User/GetAllUser").Result;
            if (responseManageAccount.IsSuccessStatusCode && responseClub.IsSuccessStatusCode 
                && responseAccount.IsSuccessStatusCode && responseUser.IsSuccessStatusCode)
            {
                int pageNumber = page ?? 1;
                int pageSize = 6;
                var jsonContentAccount = responseAccount.Content.ReadAsStringAsync().Result; 
                var jsonContentManageAccount = responseManageAccount.Content.ReadAsStringAsync().Result;
                var jsonContentClub = responseClub.Content.ReadAsStringAsync().Result;
                var jsonContentUser = responseUser.Content.ReadAsStringAsync().Result;

                var account = JsonConvert.DeserializeObject<IEnumerable<AccountDTO>>(jsonContentAccount);


                viewModel.Accounts = JsonConvert.DeserializeObject<List<AccountDTO>>(jsonContentManageAccount);
                viewModel.Clubs = JsonConvert.DeserializeObject<List<ClubDTO>>(jsonContentClub);
                viewModel.Users = JsonConvert.DeserializeObject<List<UserDTO>>(jsonContentUser);

                // Kiểm tra và lọc dữ liệu theo searchQuery nếu có
                //if (!string.IsNullOrEmpty(searchQuery))
                //{
                //    manageAccountList = manageAccountList.Where(a => a.Email.Contains(searchQuery)).ToList();
                //    clubList = clubList.Where(c => c.ClubName.Contains(searchQuery) || c.Address.Contains(searchQuery)).ToList();
                //    userList = userList.Where(u => u.FullName.Contains(searchQuery)).ToList();
                //}
                //else
                //{
                //    return View();
                //}
                // Cập nhật dữ liệu trong viewModel
                //viewModel.Accounts = manageAccountList;
                //viewModel.Clubs = clubList;
                //viewModel.Users = userList;

                // Phân trang cho từng loại tài khoản
                viewModel.PaginatedManagerAccounts = PaginatedList<AccountDTO>.CreateAsync(viewModel.Accounts, pageNumber, pageSize);
                viewModel.PaginatedClubAccounts = PaginatedList<ClubDTO>.CreateAsync(viewModel.Clubs, pageNumber, pageSize);
                viewModel.PaginatedUserAccounts = PaginatedList<UserDTO>.CreateAsync(viewModel.Users, pageNumber, pageSize);


                foreach (var club in viewModel.Clubs)
                {
                    var ownerAccount = account.FirstOrDefault(a => a.AccountID == club.AccountId);
                    if (ownerAccount != null)
                    {
                        club.AccountEmail = ownerAccount.Email;
                    }
                }
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
