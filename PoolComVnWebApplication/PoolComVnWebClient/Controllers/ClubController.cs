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
using System.Drawing.Printing;
using PoolComVnWebAPI.DTO;
using OfficeOpenXml;


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
        public IActionResult Index(int? id, int? page)
        {
            int pageNumber = page ?? 1;
            
            if (id == null)
            {
                int pageSize = 5;
                if (TempData.ContainsKey("SuccessMessage"))
                {
                    ViewBag.Success = TempData["SuccessMessage"];
                }
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
                    var paginatedClubPosts = PaginatedList<ClubPostDTO>.CreateAsync(clubPosts, pageNumber, pageSize);
                    ViewBag.ClubPost = clubPosts;
                    ViewBag.Club = club;
                    ViewBag.AccountEmail = email;
                    return View(paginatedClubPosts);
                }
                ViewBag.Club = club;
                ViewBag.AccountEmail = email;
                return View();
            }
            else
            {
                int pageSize = 6;
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
                    var paginatedClubPosts = PaginatedList<ClubPostDTO>.CreateAsync(clubPosts, pageNumber, pageSize);
                    ViewBag.ClubPost = clubPosts;
                    ViewBag.Club = club;
                    return View(paginatedClubPosts);
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
        [HttpGet]
        public IActionResult ClubPostDetails(int id,int? clubid)
        {

            if (clubid == null)
            {
                string email = HttpContext.Request.Cookies["Email"];
                var responseaccount = client.GetAsync($"{ApiUrl}/Account/GetAccountByEmail/{email}").Result;
                if (!responseaccount.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Không thể lấy thông tin tài khoản.");
                    return View();
                }
                var AccountData = responseaccount.Content.ReadAsStringAsync().Result;
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
                var response4 = client.GetAsync($"{ApiUrl}/Club/{clubid}").Result;
                if (!response4.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Không thể lấy thông tin câu lạc bộ.");
                    return View();
                }
                var ClubData = response4.Content.ReadAsStringAsync().Result;
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
                var response = client.GetAsync($"{ApiUrl}/ClubPost/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    
                    var jsonContent = response.Content.ReadAsStringAsync().Result;
                    var newsDetails = JsonConvert.DeserializeObject<ClubPostDTO>(jsonContent);
                    TempData["SuccessMessage"] = "Tạo mới bài viết" + newsDetails.Title + "thành công";
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
           
        }
        [HttpPost]
        public async Task<IActionResult> UpdateClubPost(ClubPostDTO clubPostDTO, IFormFile BannerFile)
        {

            clubPostDTO.UpdatedDate = DateTime.Now;
            if (BannerFile != null && BannerFile.Length > 0)
            {
                await DeleteFromFirebase(clubPostDTO.Title,clubPostDTO.Flyer);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(BannerFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase", fileName);

                using (FileStream memoryStream = new FileStream(filePath, FileMode.Create))
                {
                    BannerFile.CopyTo(memoryStream);


                }
                var fileStream2 = new FileStream(filePath, FileMode.Open);
                var downloadLink = await UploadFromFirebase(fileStream2, BannerFile.FileName, "ClubPost", clubPostDTO.Title, 0);
                fileStream2.Close();
                clubPostDTO.Flyer = downloadLink;
            }
            int index = 1;
            string pattern = @"<img.*?src=""(.*?)"".*?>";
            MatchCollection matches = Regex.Matches(clubPostDTO.Description, pattern);
            foreach (Match match in matches)
            {
                string src = match.Groups[1].Value;
                if (src.Contains("/o/"))
                {
                    continue;
                }
                string filenameWithoutFirebase = src.Replace("/Firebase/", "");
                string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase", filenameWithoutFirebase);
                var fileStream2 = new FileStream(absolutePath, FileMode.Open);
                var downloadLink = await UploadFromFirebase(fileStream2, filenameWithoutFirebase, "ClubPost", clubPostDTO.Title, index);
                index++;
                fileStream2.Close();
                clubPostDTO.Description = clubPostDTO.Description.Replace(src, downloadLink);
            }
            try
            {
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase");
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

            var response = client.PostAsJsonAsync($"{ApiUrl}/ClubPost/Update/{clubPostDTO.PostId}", clubPostDTO).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Chỉnh sửa bài viết" + clubPostDTO.Title + "thành công";
                return RedirectToAction("Index");
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {

                return NotFound();
            }
            else
            {

                ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật tin tức.");
                return View(clubPostDTO);
            }
        }

        public IActionResult ClubTournament(int? id)
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
                var response3 = client.GetAsync($"{ApiUrl}/Tournament/GetTournamentsByClubId?clubId={club.ClubId}").Result;
                if (response3.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.Tournament = null;
                }
                else if (response3.IsSuccessStatusCode)
                {
                    var tournamentData = response3.Content.ReadAsStringAsync().Result;
                    var tours = JsonConvert.DeserializeObject<List<TournamentDetailDTO>>(tournamentData);
                    ViewBag.Tournament = tours;
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
                var response2 = client.GetAsync($"{ApiUrl}/GetTournamentsByClubId?clubId={club.ClubId}").Result;
                if (response2.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.Tournament = null;
                }
                else if (response2.IsSuccessStatusCode)
                {
                    var tournamentData = response2.Content.ReadAsStringAsync().Result;
                    var tours = JsonConvert.DeserializeObject<List<TournamentDTO>>(tournamentData);
                    ViewBag.Tournament = tours;
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

        public IActionResult ClubSoloMatch(int? id)
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

        public IActionResult ClubSoloMatchDetail(int? id)
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

            var responseWard = client.GetAsync($"{ApiUrl}/Address/getwardsBywardCode/{club.WardCode}").Result;
            var wardData = responseWard.Content.ReadAsStringAsync().Result;
            var ward = JsonConvert.DeserializeObject<WardDTO>(wardData);
            
            var responseDistrict = client.GetAsync($"{ApiUrl}/Address/GetdistrictsByDistrictCode/{ward.DistrictCode}").Result;        
            var districtData = responseDistrict.Content.ReadAsStringAsync().Result;
            var district = JsonConvert.DeserializeObject<DistrictDTO>(districtData);

            var responseProvince = client.GetAsync($"{ApiUrl}/Address/getProvincesByProvinceCode/{district.ProvinceCode}").Result;
            var provinceData = responseProvince.Content.ReadAsStringAsync().Result;
            var province = JsonConvert.DeserializeObject<ProvinceDTO>(provinceData);
            
            ViewBag.Club = club;
            ViewBag.AccountEmail = email;
            ViewBag.Ward = ward;
            ViewBag.District = district;
            ViewBag.Province = province;
            return View(club);
        }
        [HttpPost]
        public async Task<IActionResult> ClubDetails(ClubDTO ClubDTO, IFormFile BannerFile,string ward)
        {
            if (BannerFile != null && BannerFile.Length > 0)
            {
                await DeleteFromFirebase(ClubDTO.ClubName,ClubDTO.Avatar);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(BannerFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase", fileName);

                using (FileStream memoryStream = new FileStream(filePath, FileMode.Create))
                {
                    BannerFile.CopyTo(memoryStream);


                }
                var fileStream2 = new FileStream(filePath, FileMode.Open);
                var downloadLink = await UploadFromFirebase(fileStream2, BannerFile.FileName, "Club", ClubDTO.ClubName, 0);
                fileStream2.Close();
                ClubDTO.Avatar = downloadLink;
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
            if (ward != null)
            {
                ClubDTO.WardCode = ward;
            }
            var response = await client.PostAsJsonAsync($"{ApiUrl}/Club/UpdateClub?id={ClubDTO.ClubId}", ClubDTO);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Chỉnh sửa thông tin câu lạc bộ thành công.";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi thêm tin tức.");
                return View(ClubDTO);
            }
        }    

        public IActionResult ClubTable(int? id)
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
                var response3 = client.GetAsync($"{ApiUrl}/Table/GetTablesByClubId/{club.ClubId}").Result;
                if (response3.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.Table = null;
                }
                else if (response3.IsSuccessStatusCode)
                {
                    var TableData = response3.Content.ReadAsStringAsync().Result;
                    var tables = JsonConvert.DeserializeObject<List<TableDTO>>(TableData);
                    var tableCounts = tables
                    .GroupBy(t => t.TableName)
                    .Select(g => new TableInfoViewModel
                    {
                        TableName = g.Key,
                        Quantity = g.Count(),
                        Size = g.First().Size,
                        Price = g.First().Price,
                        Image = g.First().Image,
                    })
                    .ToList();
                    ViewBag.Table = tableCounts;
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
                var response2 = client.GetAsync($"{ApiUrl}/Table/GetTablesByClubId/{club.ClubId}").Result;
                if (response2.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.Table = null;
                }
                else if (response2.IsSuccessStatusCode)
                {
                    var TableData = response2.Content.ReadAsStringAsync().Result;
                    var Tables = JsonConvert.DeserializeObject<List<TableDTO>>(TableData);
                    var tableCounts = Tables
                   .GroupBy(t => t.TableName)
                   .Select(g => new TableInfoViewModel
                   {
                       TableName = g.Key,
                       Quantity = g.Count(),
                       Size = g.First().Size,
                       Price = g.First().Price,
                       Image = g.First().Image,
                   })
                    .ToList();
                    ViewBag.Table = tableCounts;
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

        public IActionResult ClubTableManage(int? id)
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
                var response3 = client.GetAsync($"{ApiUrl}/Table/GetTablesByClubId/{club.ClubId}").Result;
                if (response3.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.Table = null;
                }
                else if (response3.IsSuccessStatusCode)
                {
                    var TableData = response3.Content.ReadAsStringAsync().Result;
                    var tables = JsonConvert.DeserializeObject<List<TableDTO>>(TableData);
                    
                    ViewBag.Table = tables;
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
                var response2 = client.GetAsync($"{ApiUrl}/Table/GetTablesByClubId/{club.ClubId}").Result;
                if (response2.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.Table = null;
                }
                else if (response2.IsSuccessStatusCode)
                {
                    var TableData = response2.Content.ReadAsStringAsync().Result;
                    var tables = JsonConvert.DeserializeObject<List<TableDTO>>(TableData);
                    ViewBag.Table = tables;
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
        [HttpPost("ImportTables")]
        public async Task<IActionResult> ImportTables(IFormFile ImportTables)
        {
            
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            try
            {
                if (ImportTables == null || ImportTables.Length <= 0)
                {
                    return BadRequest("Invalid file.");
                }

                var fileExtension = Path.GetExtension(ImportTables.FileName)?.ToLower();
                var importedTables = new List<TableDTO>();

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    using (var package = new ExcelPackage(ImportTables.OpenReadStream()))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var tableName = worksheet.Cells[row, 1].Text?.Trim();
                            var tag = worksheet.Cells[row, 2].Text?.Trim();
                            var size = worksheet.Cells[row, 3].Text?.Trim();
                            var hourlyPriceText = worksheet.Cells[row, 4].Text?.Trim();

                            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(tag) ||
                                string.IsNullOrEmpty(size) || string.IsNullOrEmpty(hourlyPriceText))
                            {
                                continue;
                            }

                            if (!int.TryParse(hourlyPriceText, out int hourlyPrice))
                            {
                                continue;
                            }

                            var table = new TableDTO
                            {
                                TableName = tableName,
                                TagName = tag,
                                Size = size,
                                Price = hourlyPrice
                            };

                            importedTables.Add(table);
                        }

                        ViewBag.ImportedTables = importedTables;
                        return View("ClubTableManage");
                    }
                }
                else
                {
                    return View("ErrorView");
                }
            }
            catch (IOException ex)
            {
                return RedirectToAction("InternalServerError", "Error", new { message = ex.Message });
            }
        }
        public IActionResult CreateSoloMatch()
        {
            return View();
        }
    }
}
