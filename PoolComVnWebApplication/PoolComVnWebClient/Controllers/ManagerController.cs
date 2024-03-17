using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PoolComVnWebClient.Common;
using PoolComVnWebClient.DTO;
using System.Net.Http.Headers;

namespace PoolComVnWebClient.Controllers
{
    public class ManagerController : Controller
    {

        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;
        public ManagerController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }
        public IActionResult Index()
        {
            var responseAccount = client.GetAsync($"{ApiUrl}"+"/Account").Result;
            var responseClub = client.GetAsync($"{ApiUrl}" + "/Club").Result;
            var responsePlayer = client.GetAsync($"{ApiUrl}" + "/Player").Result;
            if (responseAccount.IsSuccessStatusCode && responseClub.IsSuccessStatusCode && responsePlayer.IsSuccessStatusCode)
            {
                var viewModel = new ManagerDTO();


                var jsonContentAccount = responseAccount.Content.ReadAsStringAsync().Result;
                viewModel.Accounts = JsonConvert.DeserializeObject<IEnumerable<AccountDTO>>(jsonContentAccount);

                var jsonContentClub = responseClub.Content.ReadAsStringAsync().Result;
                viewModel.Clubs = JsonConvert.DeserializeObject<IEnumerable<ClubDTO>>(jsonContentClub);

                var jsonContentPlayer = responsePlayer.Content.ReadAsStringAsync().Result;
                viewModel.Players = JsonConvert.DeserializeObject<IEnumerable<PlayerDTO>>(jsonContentPlayer);

                return View(viewModel);
            }
            else
            {
                return View("Error");
            }

        }

        public IActionResult CreateManageAccount()
        {
            return View();
        }
    }
}
