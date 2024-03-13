using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PoolComVnWebClient.Common;
using PoolComVnWebClient.DTO;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace PoolComVnWebClient.Controllers
{
    public class NewsManageController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;
        private string ApiKey = FirebaseAPI.ApiKey;
        private string Bucket = FirebaseAPI.Bucket;
        private string AuthEmail = FirebaseAPI.AuthEmail;
        private string AuthPassword = FirebaseAPI.AuthPassword;

        public NewsManageController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = ApiUrl + "/News";
        }


        public  IActionResult Index()
        {
            
            var response = client.GetAsync($"{ApiUrl}").Result;
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var newsList = JsonConvert.DeserializeObject<List<NewsDTO>>(jsonContent);
                return View(newsList);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi lấy danh sách tin tức.");
                return View();
            }
        }

        public IActionResult AddNews()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadImage(List<IFormFile> files)
        {
            var filepath2 = "";
            foreach(IFormFile file  in Request.Form.Files)
                {

                var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "Firebase", file.FileName);
                using (FileStream memoryStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(memoryStream);



                }
                filepath2 = "/Firebase/"  + file.FileName;
            }
            return Json( new { url = filepath2 });
        }
        public async Task<string> UploadFromFirebase(FileStream stream, string filename, string folderName, string newsTitle, int order)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
            var cancellation = new CancellationTokenSource();
            if (order == 0)
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
                try
                {
                    await task;
                    return filename;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception was thrown : {0}", ex);
                    return null;
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
                    await task;
                    return filename;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception was thrown : {0}", ex);
                    return null;
                }
            }
        }

            [HttpPost]
        public IActionResult AddNews(NewsDTO newsDTO)
        {
            newsDTO.CreatedDate = DateTime.Now;
            newsDTO.UpdatedDate = DateTime.Now;
            newsDTO.Status = true;
            newsDTO.AccId = 1;
            string pattern = "<img\\s+src=\"([^\"]+)\"[^>]*>";
            MatchCollection matches = Regex.Matches(newsDTO.Description, pattern);
            List<string> oldImagePaths = new List<string>();
            foreach (Match match in matches)
            {
                string src = match.Groups[1].Value;  
                oldImagePaths.Add(src);
            }
            var response = client.PostAsJsonAsync($"{ApiUrl}", newsDTO).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi thêm tin tức.");
                return View(newsDTO);
            }
        }

        public IActionResult NewsDetails(int id)
        {
         
            var response = client.GetAsync($"{ApiUrl}/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var newsDetails = JsonConvert.DeserializeObject<NewsDTO>(jsonContent); 
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

        public IActionResult EditNews(int id)
        {
            
            var response = client.GetAsync($"{ApiUrl}/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var newsToEdit = JsonConvert.DeserializeObject<NewsDTO>(jsonContent); ;
                return View(newsToEdit);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                
                return NotFound();
            }
            else
            {
               
                ModelState.AddModelError(string.Empty, "Lỗi khi lấy tin tức để chỉnh sửa.");
                return View();
            }
        }

        [HttpPost]
        public IActionResult EditNews(int id, NewsDTO updatedNewsDTO)
        {
          
            var response = client.PutAsJsonAsync($"{ApiUrl}/{id}", updatedNewsDTO).Result;

            if (response.IsSuccessStatusCode)
            {
                
                return RedirectToAction("Index");
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                
                return NotFound();
            }
            else
            {
                
                ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật tin tức.");
                return View(updatedNewsDTO);
            }
        }

        [HttpPost]
        public IActionResult DeleteNews(int id)
        {
           
            var response = client.DeleteAsync($"{ApiUrl}/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                
                return RedirectToAction("Index");
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                
                return NotFound();
            }
            else
            {
               
                ModelState.AddModelError(string.Empty, "Lỗi khi xóa tin tức.");
                return View();
            }
        }
    }

}
