using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebAPI.Controllers
{
    public class TournamentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
