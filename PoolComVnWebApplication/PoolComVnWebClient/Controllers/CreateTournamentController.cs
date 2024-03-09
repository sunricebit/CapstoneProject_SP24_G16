using Microsoft.AspNetCore.Mvc;
using PoolComVnWebClient.Common;
using PoolComVnWebClient.DTO;
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
            var response = await client.PostAsJsonAsync(ApiUrl + "/Tournament", inputDTO);
            if (response.IsSuccessStatusCode)
            {

            }
            else
            {
                var status = response.StatusCode;
            }

            return RedirectToAction("StepTwoPlayerList");
        }

        [HttpGet]
        public IActionResult StepTwoPlayerList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StepTwoJoinList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StepTwoMember()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StepTwoPlayerSystem()
        {
            return View();
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
