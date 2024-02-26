using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebAPI.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
