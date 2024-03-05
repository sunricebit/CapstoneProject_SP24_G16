using Microsoft.AspNetCore.Mvc;
using PoolComVnWebClient.Common;
using System.Net.Http.Headers;

namespace PoolComVnWebClient.Controllers
{
    public class NewsManageController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;
        public NewsManageController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = ApiUrl + "/News";
        }
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
