using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebAPI.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
