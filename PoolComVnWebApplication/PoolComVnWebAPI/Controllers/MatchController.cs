using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebAPI.Controllers
{
    public class MatchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
