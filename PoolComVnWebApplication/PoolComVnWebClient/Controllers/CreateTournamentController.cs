using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebClient.Common;
using PoolComVnWebClient.DTO;
using System.Collections.Generic;
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
        public IActionResult StepOneCreateTournament()
        {
            return View();
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
            }

            return RedirectToAction("InternalServerError", "Error");
        }

        [HttpGet]
        public IActionResult StepTwoPlayerList()
        {
            return View();
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
        public IActionResult StepThreeAddTable()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StepFourAddBanner()
        {
            return View();
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
