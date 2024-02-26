using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace PoolComVnWebClient.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public HttpResponseMessage Login()
        //{

        //}
    }
}
