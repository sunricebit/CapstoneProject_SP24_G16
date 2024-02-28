using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebClient.Views.Home
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
    }
}
