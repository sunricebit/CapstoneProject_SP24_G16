﻿using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using PoolComVnWebClient.Common;
using PoolComVnWebClient.DTO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PoolComVnWebClient.Controllers
{
    public class CreateTournamentController : Controller
    {
        private readonly HttpClient client;
        private string ApiUrl = Constant.ApiUrl;
        private string ApiKey = FirebaseAPI.ApiKey;
        private string Bucket = FirebaseAPI.Bucket;
        private string AuthEmail = FirebaseAPI.AuthEmail;
        private string AuthPassword = FirebaseAPI.AuthPassword;

        public CreateTournamentController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = ApiUrl + "/CreateTournament";
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StepOneCreateTournament()
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            List<int> rolesAccess = new List<int>();

            // Thêm các role được access
            rolesAccess.Add(Constant.BusinessRole);
            var response = await client.PostAsJsonAsync(Constant.ApiUrl + "/Authorization/CheckAuthorization", rolesAccess);
            if (response.IsSuccessStatusCode)
            {
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            else
            {
                return RedirectToAction("NotAccess", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> StepOneCreateTournament(CreateTournamentInputDTO inputDTO)
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            List<string> errors = new List<string>();

            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values)
                {
                    foreach (var modelError in error.Errors)
                    {
                        errors.Add(modelError.ErrorMessage);
                    }
                }

            }

            if (inputDTO.StartTime <= DateTime.Now)
            {
                errors.Add("Thời gian bắt đầu phải lớn hơn thời gian hiện tại.");
            }

            if (inputDTO.EndTime < inputDTO.StartTime)
            {
                errors.Add("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
            }

            if (inputDTO.RegistrationDeadline > inputDTO.StartTime)
            {
                errors.Add("Thời hạn đăng ký trước thời gian bắt đầu.");
            }

            if (errors.Count > 0)
            {
                ViewBag.Error = errors;
                return View();
            }
            var response = await client.PostAsJsonAsync(ApiUrl + "/CreateTourStOne", inputDTO);
            if (response.IsSuccessStatusCode)
            {
                int tourId = await response.Content.ReadFromJsonAsync<int>();
                ViewBag.TourId = tourId;
                return RedirectToAction("StepTwoAddBanner", "CreateTournament", new { tourId = tourId });
            }
            else
            {
                var status = response.StatusCode;
                return RedirectToAction("InternalServerError", "Error");
            }
        }

        [HttpGet("CreateTournament/StepTwoAddBanner/{tourID}")]
        public async Task<IActionResult> StepTwoAddBanner(int tourId)
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            List<int> rolesAccess = new List<int>();

            // Thêm các role được access
            rolesAccess.Add(Constant.BusinessRole);
            var response = await client.PostAsJsonAsync(Constant.ApiUrl + "/Authorization/CheckAuthorization", rolesAccess);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.TourId = tourId;
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            else
            {
                return RedirectToAction("NotAccess", "Error");
            }
        }

        [HttpPost("CreateTournament/StepTwoAddBanner/{tourID}")]
        public async Task<IActionResult> StepTwoAddBanner(IFormFile banner, int tourId)
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);

            StepTwoAddBannerDTO BannerDTO = new StepTwoAddBannerDTO();
            var bannerContent = new MultipartFormDataContent();
            try
            {

                if (banner != null && banner.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(banner.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase", fileName);

                    using (FileStream memoryStream = new FileStream(filePath, FileMode.Create))
                    {
                        banner.CopyTo(memoryStream);


                    }
                    var fileStream2 = new FileStream(filePath, FileMode.Open);
                    var downloadLink = await UploadFromFirebase(fileStream2, banner.FileName);
                    fileStream2.Close();
                    string Flyer = downloadLink;
                    System.IO.File.Delete(filePath);
                    BannerDTO.Flyer = Flyer;
                }
                BannerDTO.TourId = tourId;

                var response = await client.PostAsJsonAsync(ApiUrl + "/CreateTourStTwo", BannerDTO);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.TourId = await response.Content.ReadFromJsonAsync<int>();
                    return RedirectToAction("StepThreeReview", new { tourId = tourId });
                }
                else
                {
                    var status = response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("InternalServerError", "Error");
            }
            finally
            {

                bannerContent?.Dispose();
            }

            return RedirectToAction("InternalServerError", "Error");
        }

        [HttpGet]
        public async Task<IActionResult> StepThreeReview(int? tourId)
        {
            TournamentDetailDTO tourDetail;
            var responseGetTourdetail = await client.GetAsync(Constant.ApiUrl + "/Tournament/GetTournament?tourId=" + tourId);
            if (responseGetTourdetail.IsSuccessStatusCode)
            {
                tourDetail = await responseGetTourdetail.Content.ReadFromJsonAsync<TournamentDetailDTO>();
                ViewBag.TournamentDetail = tourDetail;
            }
            else
            {
                var status = responseGetTourdetail.StatusCode;
                return RedirectToAction("InternalServerError", "Error");
            }

            var responseGetLstPlayer = await client.GetAsync(Constant.ApiUrl + "/Player" + "/GetNumberPlayerExBotByTourId?tourId=" + tourId);
            if (responseGetLstPlayer.IsSuccessStatusCode)
            {
                int numberOfPlayer = await responseGetLstPlayer.Content.ReadFromJsonAsync<int>();
                ViewBag.NumberOfPlayer = numberOfPlayer;
                ViewBag.TourId = tourId;
                return View();
            }
            else
            {
                var status = responseGetLstPlayer.StatusCode;
                return RedirectToAction("InternalServerError", "Error");
            }
        }

        [HttpGet("CreateTournament/StepFourAddTable/{tourID}")]
        public async Task<IActionResult> StepFourAddTable(int tourId)
        {
            ViewBag.TourId = tourId;
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            var response = await client.GetFromJsonAsync<IEnumerable<TableDTO>>(Constant.ApiUrl + "/Table" + "/GetAllTablesForTournament");
            return View(response);
        }

        [HttpPost("StepFourAddTable/{tourId}")]
        public async Task<IActionResult> StepFourAddTable(int tourId, List<int> lstTableId)
        {
            ViewBag.TourId = tourId;
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            var response = await client.PostAsJsonAsync(Constant.ApiUrl + "/Table" + "/AddTableToTournament", lstTableId);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("StepFivePlayerList", "CreateTournament", new { tourId = tourId });
            }
            else
            {
                var status = response.StatusCode;
                return RedirectToAction("InternalServerError", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> StepFivePlayerList(int? tourId)
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            List<int> rolesAccess = new List<int>();

            // Thêm các role được access
            rolesAccess.Add(Constant.BusinessRole);
            var response = await client.PostAsJsonAsync(Constant.ApiUrl + "/Authorization/CheckAuthorization", rolesAccess);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.TourId = tourId;
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            else
            {
                return RedirectToAction("NotAccess", "Error");
            }
        }

        [HttpPost("ImportPlayers/{id}")]
        public async Task<IActionResult> ImportPlayers(IFormFile ImportPlayers, int id)
        {
            ViewBag.TourId = id;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            try
            {
                if (ImportPlayers == null || ImportPlayers.Length <= 0)
                {
                    return BadRequest("Invalid file.");
                }

                var fileExtension = Path.GetExtension(ImportPlayers.FileName)?.ToLower();
                var importedPlayers = new List<PlayerDTO>();
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {

                    using (var package = new ExcelPackage(ImportPlayers.OpenReadStream()))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var playerName = worksheet.Cells[row, 1].Text?.Trim();
                            var countryName = worksheet.Cells[row, 2].Text?.Trim();
                            var phoneNumber = worksheet.Cells[row, 3].Text?.Trim();
                            var email = worksheet.Cells[row, 4].Text?.Trim();
                            var levelText = worksheet.Cells[row, 5].Text?.Trim();

                            var feeText = worksheet.Cells[row, 6].Text?.Trim();

                            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(countryName) ||
                                string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(levelText) ||
                                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(feeText))
                            {
                                continue;
                            }
                            bool fee;
                            if (feeText == "Rồi")
                            {
                                fee = true;

                            }
                            else
                            {
                                fee = false;
                            }


                            if (!int.TryParse(levelText, out int level))
                            {
                                continue;
                            }

                            var player = new PlayerDTO
                            {
                                PlayerId = row,
                                PlayerName = playerName,
                                CountryName = countryName,
                                PhoneNumber = phoneNumber,
                                Email = email,
                                Level = level,
                                IsPayed = fee
                            };

                            importedPlayers.Add(player);
                        }
                        ViewBag.ImportedPlayers = importedPlayers;

                    }
                    return View("StepFivePlayerList");
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

        public async Task<string> UploadFromFirebase(FileStream stream, string filename)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                }
                ).Child("Tournaments")
                 .Child(filename)
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



        [HttpGet]
        public async Task<IActionResult> StepFiveJoinList()
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            List<int> rolesAccess = new List<int>();

            // Thêm các role được access
            rolesAccess.Add(Constant.BusinessRole);
            var response = await client.PostAsJsonAsync(Constant.ApiUrl + "/Authorization/CheckAuthorization", rolesAccess);
            if (response.IsSuccessStatusCode)
            {
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            else
            {
                return RedirectToAction("NotAccess", "Error");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StepFiveMember()
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            List<int> rolesAccess = new List<int>();

            // Thêm các role được access
            rolesAccess.Add(Constant.BusinessRole);
            var response = await client.PostAsJsonAsync(Constant.ApiUrl + "/Authorization/CheckAuthorization", rolesAccess);
            if (response.IsSuccessStatusCode)
            {
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            else
            {
                return RedirectToAction("NotAccess", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> StepFivePlayerSystem()
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            var response = await client.GetFromJsonAsync<IEnumerable<PlayerDTO>>("https://localhost:5000/api/Player");
            var listplayer = response.ToList();
            return View(listplayer);
        }

        [HttpGet]
        public async Task<IActionResult> StepSixArrange(int tourId)
        {
            int numberOfPlayer = await client
                .GetFromJsonAsync<int>(Constant.ApiUrl + "/Player" + "/GetNumberPlayerByTourId?tourId=" + tourId);
            int maxNumberOfTournament = await client
                .GetFromJsonAsync<int>(Constant.ApiUrl + "/Tournament/GetTourMaxNumberOfPlayer?tourId=" + tourId);
            int? knockOutNumber = await client
                .GetFromJsonAsync<int?>(Constant.ApiUrl + "/Tournament/GetTourKnockoutNumber?tourId=" + tourId);
            int logOfMaxNumberOfTournament = (int)Math.Log2(maxNumberOfTournament);
            int numberPlayerCheck = (int)Math.Pow(2, logOfMaxNumberOfTournament - 1);
            int numberPlayerRecommend = maxNumberOfTournament;
            while (numberOfPlayer < numberPlayerCheck)
            {
                numberPlayerRecommend = numberPlayerCheck;
                logOfMaxNumberOfTournament--;
                numberPlayerCheck = (int)Math.Pow(2, logOfMaxNumberOfTournament - 1);
            }

            ViewBag.MaxNumberOfTournament = maxNumberOfTournament;
            ViewBag.NumberOfPlayer = numberOfPlayer;
            ViewBag.NumberRecommend = numberPlayerRecommend;
            ViewBag.IsDouble = false;

            if (numberPlayerRecommend == maxNumberOfTournament)
            {
                await client
                .GetAsync(Constant.ApiUrl + "/Player/GenerateBotInTour?tourId=" + tourId);
            }

            if (knockOutNumber.HasValue) 
            {
                ViewBag.KnockOutNumber = knockOutNumber;
                ViewBag.IsDouble = true;
            }
            ViewBag.TourId = tourId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserRandom(int tourId)
        {
            int maxNumberOfTournament = await client
                .GetFromJsonAsync<int>(Constant.ApiUrl + "/Tournament/GetTourMaxNumberOfPlayer?tourId=" + tourId);
            ViewBag.MaxNumberOfTournament = maxNumberOfTournament;
            ViewBag.TourID = tourId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserCustom(int tourId)
        {
            int maxNumberOfTournament = await client
                .GetFromJsonAsync<int>(Constant.ApiUrl + "/Tournament/GetTourMaxNumberOfPlayer?tourId=" + tourId);
            ViewBag.MaxNumberOfTournament = maxNumberOfTournament;
            ViewBag.TourID = tourId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SystemRandom(int tourId)
        {
            int maxNumberOfTournament = await client
                .GetFromJsonAsync<int>(Constant.ApiUrl + "/Tournament/GetTourMaxNumberOfPlayer?tourId=" + tourId);
            int? knockOutNumber = await client
                .GetFromJsonAsync<int?>(Constant.ApiUrl + "/Tournament/GetTourKnockoutNumber?tourId=" + tourId);
            ViewBag.MaxNumberOfTournament = maxNumberOfTournament;
            ViewBag.KnockOutNumber = knockOutNumber;
            ViewBag.TourID = tourId;
            return View();
        }

        public IActionResult SystemSingleRandom(int tourId)
        {
            ViewBag.TourID = tourId;
            return View();
        }

        [HttpPost]
        public IActionResult CreateTournament()
        {
            return View();
        }
    }
}
