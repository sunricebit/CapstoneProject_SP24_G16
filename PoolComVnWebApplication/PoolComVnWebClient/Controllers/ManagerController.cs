using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebClient.Controllers
{
    public class ManagerController : Controller
    {
        //manage account
        public IActionResult Index()
        {
            return View();
        }
    }
}
