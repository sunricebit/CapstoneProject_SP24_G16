using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebClient.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("NotAccess")]
        public IActionResult NotAccess()
        {
            return View();
        }

        [HttpGet("404")]
        public IActionResult NotFound()
        {
            return View();
        }

        [HttpGet("InternalServerError")]
        public IActionResult InternalServerError(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
        }
    }
}
