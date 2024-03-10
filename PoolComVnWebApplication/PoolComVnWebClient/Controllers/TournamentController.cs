using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebClient.DTO;
using PoolComVnWebClient.Common;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace PoolComVnWebClient.Controllers
{
    public class TournamentController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;

        public TournamentController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = ApiUrl + "/Tournament";
        }

        [HttpGet("{tourId}")]
        public async Task<IActionResult> TournamentDetail(int tourId)
        {
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TournamentList()
        {
            var tokenFromCookie = HttpContext.Request.Cookies["TokenJwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromCookie);
            var response = await client.GetAsync(ApiUrl + "/GetAllTour");
            if (response.IsSuccessStatusCode)
            {
                List<TournamentOutputDTO> lstTour = await response.Content.ReadFromJsonAsync<List<TournamentOutputDTO>>();
                return View(lstTour);
            }
            else
            {
                var status = response.StatusCode;
            }

            return RedirectToAction("InternalServerError", "Error");
        }

        public IActionResult TournamentBracket()
        {
            return View();
        }

        public IActionResult TournamentMatchList()
        {
            return View();
        }

        public IActionResult TournamentUpcoming()
        {
            return View();
        }

        public IActionResult TournamentPlayers()
        {
            return View();
        }

        //Tournament detail for club and manage of club
        public IActionResult TournamentMatchListForManager()
        {
            return View();
        }

        public IActionResult TournamentDetailForManager()
        {
            return View();
        }

        public IActionResult TournamentBracketForManager() {
            return View();
        }
        //[HttpPost("/Create")]
        //public
    }
}
