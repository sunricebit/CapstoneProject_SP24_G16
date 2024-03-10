using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebClient.Controllers
{
    public class TournamentController : Controller
    {
        public IActionResult Index()
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
    }
}
