using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using PoolComVnWebClient.Common;
using Newtonsoft.Json;
using PoolComVnWebClient.DTO;
using System.Net;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace PoolComVnWebClient.Controllers
{
    public class ClubController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;
        private string ApiKey = FirebaseAPI.ApiKey;
        private string Bucket = FirebaseAPI.Bucket;
        private string AuthEmail = FirebaseAPI.AuthEmail;
        private string AuthPassword = FirebaseAPI.AuthPassword;
        public ClubController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }
        public IActionResult Index(int? id)
        {
            if (id == null)
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
                var response2 = client.GetAsync($"{ApiUrl}/Club/GetClubByAccountId/?accountID={account.AccountID}").Result;
                if (!response2.IsSuccessStatusCode)
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
                var response3 = client.GetAsync($"{ApiUrl}/ClubPost/GetByClubId/{club.ClubId}").Result;
                if (response3.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.ClubPost = null;
                }
                else if (response3.IsSuccessStatusCode)
                {
                    var clubPostData = response3.Content.ReadAsStringAsync().Result;
                    var clubPosts = JsonConvert.DeserializeObject<List<ClubPostDTO>>(clubPostData);
                    ViewBag.ClubPost = clubPosts;
                }
                ViewBag.Club = club;
                ViewBag.AccountEmail = email;
                return View();
            }
            else
            {
                var response = client.GetAsync($"{ApiUrl}/Club/{id}").Result;
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Không thể lấy thông tin câu lạc bộ.");
                    return View();
                }
                var ClubData = response.Content.ReadAsStringAsync().Result;
                var club = JsonConvert.DeserializeObject<ClubDTO>(ClubData);
                ViewBag.Club = club;
                var response2 = client.GetAsync($"{ApiUrl}/ClubPost/GetByClubId/{club.ClubId}").Result;
                if (response2.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.ClubPost = null;
                }
                else if (response2.IsSuccessStatusCode)
                {
                    var clubPostData = response2.Content.ReadAsStringAsync().Result;
                    var clubPosts = JsonConvert.DeserializeObject<List<ClubPostDTO>>(clubPostData);
                    ViewBag.ClubPost = clubPosts;
                }
                var response3 = client.GetAsync($"{ApiUrl}/Account/GetAccountById/{club.AccountId}").Result;
                if (!response3.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Không thể lấy thông tin tài khoản.");
                    return View();
                }
                var AccountData = response3.Content.ReadAsStringAsync().Result;
                var account = JsonConvert.DeserializeObject<AccountDTO>(AccountData);
                ViewBag.AccountEmail = account.Email;
                return View();
            }

        }
        public IActionResult AddPost(int clubid)
        {
            ViewBag.ClubID = clubid;
            return View();
        }
        [HttpPost]
        public ActionResult UploadImage(List<IFormFile> files)
        {
            var filepath2 = "";
            foreach (IFormFile file in Request.Form.Files)
            {

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase", file.FileName);
                using (FileStream memoryStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(memoryStream);

                }
                filepath2 = "/Firebase/" + file.FileName;
            }
            return Json(new { url = filepath2 });
        }
        public async Task<string> UploadFromFirebase(FileStream stream, string filename, string folderName, string newsTitle, int order)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
            var cancellation = new CancellationTokenSource();
            if (order == 0)
            {
                if (!string.IsNullOrEmpty(newsTitle))
                {
                    var task = new FirebaseStorage(
                   Bucket,
                   new FirebaseStorageOptions
                   {
                       AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                       ThrowOnCancel = true
                   }
               ).Child(folderName)
               .Child(newsTitle)
                .Child($"Banner")
                .PutAsync(stream, cancellation.Token);
                    string link = await task;
                    return link;
                }
                 else
                {
                    var task = new FirebaseStorage(
                   Bucket,
                   new FirebaseStorageOptions
                   {
                       AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                       ThrowOnCancel = true
                   }
               ).Child(folderName)
                .Child($"Banner")
                .PutAsync(stream, cancellation.Token);
                    string link = await task;
                    return link;
                }  
             
                   


            }
            else
            {
                var orderedFileName = $"Image{order}{Path.GetExtension(stream.Name)}";
                var task = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                ).Child(folderName)
                .Child(newsTitle)
                 .Child(orderedFileName)
                 .PutAsync(stream, cancellation.Token);
                try
                {
                    string link = await task;
                    return link;


                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception was thrown : {0}", ex);
                    return null;
                }
            }
        }
        public async Task DeleteFromFirebase(string title, string filename)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

                var cancellation = new CancellationTokenSource();
                var storage = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                );
                var folderPath = $"News/{title}/{filename}";


                await storage.Child(folderPath).DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred during deletion: {0}", ex);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddPost(ClubPostDTO ClubPostDTO, IFormFile BannerFile,int clubid)
        {
            ClubPostDTO.CreatedDate = DateTime.Now;
            ClubPostDTO.UpdatedDate = DateTime.Now;
            ClubPostDTO.Status = true;
            ClubPostDTO.ClubId = clubid;
            if (BannerFile != null && BannerFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(BannerFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase", fileName);

                using (FileStream memoryStream = new FileStream(filePath, FileMode.Create))
                {
                    BannerFile.CopyTo(memoryStream);


                }
                var fileStream2 = new FileStream(filePath, FileMode.Open);
                var downloadLink = await UploadFromFirebase(fileStream2, BannerFile.FileName, "ClubPost", ClubPostDTO.Title, 0);
                fileStream2.Close();
                ClubPostDTO.Flyer = downloadLink;
            }
            int index = 1;
            string pattern = @"<img.*?src=""(.*?)"".*?>";
            MatchCollection matches = Regex.Matches(ClubPostDTO.Description, pattern);
            foreach (Match match in matches)
            {
                string src = match.Groups[1].Value;
                string filenameWithoutFirebase = src.Replace("/Firebase/", "");
                string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase", filenameWithoutFirebase);
                var fileStream2 = new FileStream(absolutePath, FileMode.Open);
                var downloadLink = await UploadFromFirebase(fileStream2, filenameWithoutFirebase, "ClubPost", ClubPostDTO.Title, index);
                index++;
                fileStream2.Close();
                ClubPostDTO.Description = ClubPostDTO.Description.Replace(src, downloadLink);
            }
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase");

            try
            {

                string[] filePaths = Directory.GetFiles(directoryPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Console.WriteLine("All images in the directory have been deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting images: {ex.Message}");
            }
            ClubPostDTO.PostId = 1;
            var response = await client.PostAsJsonAsync($"{ApiUrl}/ClubPost/Add", ClubPostDTO);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi thêm tin tức.");
                return View(ClubPostDTO);
            }
        }

        public IActionResult ClubTournament()
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
            var response2 = client.GetAsync($"{ApiUrl}/Club/GetClubByAccountId/?accountID={account.AccountID}").Result;
            if (!response2.IsSuccessStatusCode)
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
            ViewBag.Club = club;
            ViewBag.AccountEmail = email;
            return View();
        }

        public IActionResult ClubSoloMatch()
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
            var response2 = client.GetAsync($"{ApiUrl}/Club/GetClubByAccountId/?accountID={account.AccountID}").Result;
            if (!response2.IsSuccessStatusCode)
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
            ViewBag.Club = club;
            ViewBag.AccountEmail = email;
            return View();
        }

        public IActionResult ClubSoloMatchDetail()
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
            var response2 = client.GetAsync($"{ApiUrl}/Club/GetClubByAccountId/?accountID={account.AccountID}").Result;
            if (!response2.IsSuccessStatusCode)
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
            ViewBag.Club = club;
            ViewBag.AccountEmail = email;
            return View();
        }

        public IActionResult CreateClub()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateClub(ClubDTO clubDTO, IFormFile BannerFile, string ward)
        {
            string email = HttpContext.Request.Cookies["Email"];
            var response = client.GetAsync($"{ApiUrl}/Account/GetAccountByEmail/{email}").Result;
            var AccountData = response.Content.ReadAsStringAsync().Result;
            var account = JsonConvert.DeserializeObject<AccountDTO>(AccountData);
            clubDTO.AccountId = account.AccountID;
            clubDTO.Status = 1;
            clubDTO.WardCode = ward;
            if (BannerFile != null && BannerFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(BannerFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase", fileName);

                using (FileStream memoryStream = new FileStream(filePath, FileMode.Create))
                {
                    BannerFile.CopyTo(memoryStream);


                }
                var fileStream2 = new FileStream(filePath, FileMode.Open);
                var downloadLink = await UploadFromFirebase(fileStream2, BannerFile.FileName, "Club", clubDTO.ClubName, 0);
                fileStream2.Close();
                clubDTO.Avatar = downloadLink;
            }
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase");

            try
            {

                string[] filePaths = Directory.GetFiles(directoryPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Console.WriteLine("All images in the directory have been deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting images: {ex.Message}");
            }
            clubDTO.ClubId = 1;
            var response2 = await client.PostAsJsonAsync($"{ApiUrl}/Club/Add", clubDTO);

            if (response2.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi thêm tin tức.");
                return View(clubDTO);
            }
        }

        public IActionResult ClubDetails(int id)
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
            var response2 = client.GetAsync($"{ApiUrl}/Club/GetClubByAccountId/?accountID={account.AccountID}").Result;
            if (!response2.IsSuccessStatusCode)
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
            ViewBag.Club = club;
            ViewBag.AccountEmail = email;
            return View();
        }

        public IActionResult ClubTable()
        {
            return View();
        }

        public IActionResult ClubTableManage()
        {
            return View();
        }
    }
}
