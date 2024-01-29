using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebClient.Controllers
{
    public class ForgotPasswordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
