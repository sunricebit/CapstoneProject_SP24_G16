using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebClient.Controllers
{
    public class ClubController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ClubTournament()
        {
            return View();
        }
    }
}
