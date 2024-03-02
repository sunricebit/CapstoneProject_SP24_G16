using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebClient.Controllers
{
    public class NewsManageController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult AddNews()
        {
            return View();
        }
        public IActionResult NewsDetails()
        {
            return View();
        }

    }
}
