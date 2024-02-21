using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
