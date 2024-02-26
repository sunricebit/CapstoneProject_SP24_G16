using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebAPI.Controllers
{
    public class SoloMatchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
