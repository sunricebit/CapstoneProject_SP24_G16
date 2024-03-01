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

        public IActionResult TournamentUpcomingMatchList()
        {
            return View();
        }

        public IActionResult TournamentPlayers()
        {
            return View();
        }
    }
}
