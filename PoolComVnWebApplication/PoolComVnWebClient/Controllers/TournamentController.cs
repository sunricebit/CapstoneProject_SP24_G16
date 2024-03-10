using Microsoft.AspNetCore.Mvc;
using PoolComVnWebClient.Common;
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

        [HttpGet]
        public async Task<IActionResult> TournamentDetail(int tourId)
        {
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Tournament()
        {

            return View();
        }

        public IActionResult TournamentBracket()
        {
            return View();
        }

        public IActionResult TournamentMatchList()
        {
            return View();
        }

        public IActionResult TournamentUpcomingMatchList()
        {
            return View();
        }

        public IActionResult TournamentPlayers()
        {
            return View();
        }

        //[HttpPost("/Create")]
        //public
    }
}
