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
        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;

        public CreateTournamentController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = ApiUrl + "/Tournament";
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
                return RedirectToAction("Forbidden", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> StepOneCreateTournament(CreateTournamentInputDTO inputDTO)
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            var response = await client.PostAsJsonAsync(ApiUrl + "/CreateTourStOne", inputDTO);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.TourId = await response.Content.ReadFromJsonAsync<int>();
                return View("StepTwoPlayerList");
            }
            else
            {
                var status = response.StatusCode;
                return RedirectToAction("InternalServerError", "Error");
            }
        }

        [HttpGet]
        public IActionResult StepTwoPlayerList()
        {
            return View();
        }

        [HttpPost("ImportPlayers")]
        public async Task<IActionResult>  ImportPlayers(IFormFile ImportPlayers, int tourId)
        {
            ViewBag.TourId = tourId;
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
                    return View("StepTwoPlayerList");
                }
                else
                {
                    return View("ErrorView");
                }

            }
            catch (IOException ex)
            {
                return View();
            }

        }
       

        [HttpGet]
        public async Task<IActionResult> StepTwoJoinList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StepTwoMember()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StepTwoPlayerSystem()
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            var response = await client.GetFromJsonAsync<IEnumerable<PlayerDTO>>("https://localhost:5000/api/Player");
            var listplayer = response.ToList();
            return View(listplayer);
        }

        [HttpGet]
        public async Task<IActionResult> StepThreeAddTable(int tourId)
        {
            ViewBag.TourId = tourId;
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            var response = await client.GetFromJsonAsync<IEnumerable<TableDTO>>("https://localhost:5000/api/Table/GetAllTablesForClub");
            var listtable= response.ToList();
            return View(listtable);
        }

        [HttpGet]
        public IActionResult StepFourAddBanner(int tourID)
        {
            ViewBag.TourId = tourID;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StepFourAddBanner(IFormFile banner, int tourID)
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            var bannerContent = new MultipartFormDataContent();
            try
            {
                var stream = new MemoryStream();
                
                    await banner.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    bannerContent.Add(new StreamContent(stream), "banner", banner.FileName);

                  
                

                bannerContent.Add(new StringContent(tourID.ToString()), "tourID");

                var response = await client.PostAsync(ApiUrl + "/CreateTourStFour", bannerContent);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.TourId = await response.Content.ReadFromJsonAsync<int>();
                    return View("StepFiveArrange");
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
        public IActionResult StepFiveArrange()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SystemRandom()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UserRandom()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UserCustom()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StepSixReview()
        {
            return View();
        }


    }
}
