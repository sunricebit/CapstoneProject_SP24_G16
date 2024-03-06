using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebClient.Controllers
{
    public class CreateTournamentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StepOneCreateTournament()
        {
            return View();
        }

        public IActionResult StepTwoPlayerList()
        {
            return View();
        }

        public IActionResult StepTwoJoinList()
        {
            return View();
        }

        public IActionResult StepTwoMember()
        {
            return View();
        }

        public IActionResult StepTwoPlayerSystem()
        {
            return View();
        }
    }
}
