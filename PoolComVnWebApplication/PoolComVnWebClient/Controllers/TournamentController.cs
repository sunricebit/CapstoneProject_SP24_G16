using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebClient.DTO;
using PoolComVnWebClient.Common;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace PoolComVnWebClient.Controllers
{
    public class TournamentController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrl = Constant.ApiUrl;

        public TournamentController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = ApiUrl + "/Tournament";
        }

        [HttpGet]
        public async Task<IActionResult> TournamentList(int? page, string searchQuery, string? gameType, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                int pageNumber = page ?? 1;
                int pageSize = 6;
                ViewBag.SearchQuery = searchQuery;
                ViewBag.GameType = gameType;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                List<TournamentOutputDTO> tournamentsList = null;

                // Kiểm tra nếu có thông số lọc được cung cấp
                if (!string.IsNullOrEmpty(gameType) || startDate.HasValue || endDate.HasValue)
                {
                    tournamentsList = await GetFilteredTournamentsListAsync(gameType, startDate, endDate);
                }
                else if (!string.IsNullOrEmpty(searchQuery))
                {
                    tournamentsList = await GetSearchTournamentsListAsync(searchQuery);
                    
                }
                else
                {
                    var response = await client.GetAsync(ApiUrl + "/GetAllTour");
                    if (response.IsSuccessStatusCode)
                    {
                        tournamentsList = await response.Content.ReadFromJsonAsync<List<TournamentOutputDTO>>();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Lỗi khi lấy danh sách giải đấu từ API");
                        return RedirectToAction("InternalServerError", "Error");
                    }

                }
                if (tournamentsList != null)
                {
                    var paginatedTournamentsList = PaginatedList<TournamentOutputDTO>.CreateAsync(tournamentsList, pageNumber, pageSize);
                    return View(paginatedTournamentsList);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Lỗi khi lấy danh sách giải đấu từ API");
                    return RedirectToAction("InternalServerError", "Error");
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi kết nối đến API: " + ex.Message);
                return RedirectToAction("InternalServerError", "Error");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi lấy danh sách giải đấu: " + ex.Message);
                return RedirectToAction("InternalServerError", "Error");
            }
        }

        private async Task<List<TournamentOutputDTO>> GetFilteredTournamentsListAsync(string gameType, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string apiUrl = ApiUrl + "/FilterTournaments?";
                if (!string.IsNullOrEmpty(gameType) && gameType != "Thể loại")
                    apiUrl += $"gameTypeName={gameType}&";
                if (startDate.HasValue)
                    apiUrl += $"startDate={startDate.Value.ToString("MM-dd-yyyy")}&";
                if (endDate.HasValue)
                    apiUrl += $"endDate={endDate.Value.ToString("MM-dd-yyyy")}&";

                // Gửi yêu cầu tới API filter
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    List<TournamentOutputDTO> lstTour = await response.Content.ReadFromJsonAsync<List<TournamentOutputDTO>>();
                    return lstTour;
                }
                else
                {
                    var status = response.StatusCode;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while getting filtered tournaments list: {ex.Message}");
                return null;
            }
        }


        private async Task<List<TournamentOutputDTO>> GetSearchTournamentsListAsync(string searchQuery)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    var response = await client.GetAsync(ApiUrl + "/GetAllTour");
                    if (response.IsSuccessStatusCode)
                    {
                        List<TournamentOutputDTO> lstTour = await response.Content.ReadFromJsonAsync<List<TournamentOutputDTO>>();
                        return lstTour;
                    }
                    else
                    {
                        var status = response.StatusCode;
                        return null;
                    }
                }
                else
                {
                    var response = await client.GetAsync(ApiUrl + $"/SearchTournament?searchQuery={searchQuery}");
                    if (response.IsSuccessStatusCode)
                    {
                        List<TournamentOutputDTO> lstTour = await response.Content.ReadFromJsonAsync<List<TournamentOutputDTO>>();
                        return lstTour;
                    }
                    else
                    {
                        var status = response.StatusCode;
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while getting search tournaments list: {ex.Message}");
                return null;
            }
        }


        [HttpGet]
        public IActionResult TournamentBracket(int tourId)
        {
            ViewBag.TourId = tourId;
            return View();
        }

        public IActionResult TournamentSingleBracket(int tourId)
        {
            return View();
        }

        public IActionResult TournamentMatchList()
        {
            return View();
        }

        public IActionResult TournamentUpcoming()
        {
            return View();
        }

        public IActionResult TournamentPlayers()
        {
            return View();
        }
        public IActionResult TournamentMatchListForManager()
        {
            return View();
        }

        public async Task<IActionResult> TournamentDetailForManager(int tourId)
        {
            TournamentDetailDTO tourDetail = new TournamentDetailDTO();
            var responseGetTourdetail = await client.GetAsync(ApiUrl + "/GetTournament?tourId=" + tourId);
            if (responseGetTourdetail.IsSuccessStatusCode)
            {
                tourDetail = await responseGetTourdetail.Content.ReadFromJsonAsync<TournamentDetailDTO>();
                ViewBag.TournamentDetail = tourDetail;
            }
            else
            {
                var status = responseGetTourdetail.StatusCode;
                return RedirectToAction("InternalServerError", "Error");
            }
            List<PlayerDTO> lstPlayer;
            var responseGetLstPlayer = await client.GetAsync(Constant.ApiUrl + "/Player" + "/GetPlayerByTourId?tourId=" + tourId);
            if (responseGetLstPlayer.IsSuccessStatusCode)
            {
                lstPlayer = await responseGetLstPlayer.Content.ReadFromJsonAsync<List<PlayerDTO>>();
                ViewBag.PlayerLst = lstPlayer;
            }
            else
            {
                var status = responseGetTourdetail.StatusCode;
                return RedirectToAction("InternalServerError", "Error");
            }
            return View();
        }

        public IActionResult TournamentBracketForManager()
        {
            return View();
        }

        public IActionResult TournamentSingleBracketForManager()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TournamentDetail(int tourId)
        {
            TournamentDetailDTO tourDetail = new TournamentDetailDTO();
            var responseGetTourdetail = await client.GetAsync(ApiUrl + "/GetTournament?tourId=" + tourId);
            if (responseGetTourdetail.IsSuccessStatusCode)
            {
                tourDetail = await responseGetTourdetail.Content.ReadFromJsonAsync<TournamentDetailDTO>();
                ViewBag.TournamentDetail = tourDetail;
            }
            else
            {
                var status = responseGetTourdetail.StatusCode;
                return RedirectToAction("InternalServerError", "Error");
            }
            List<PlayerDTO> lstPlayer;
            var responseGetLstPlayer = await client.GetAsync(Constant.ApiUrl + "/Player" + "/GetPlayerByTourId?tourId=" + tourId);
            if (responseGetLstPlayer.IsSuccessStatusCode)
            {
                lstPlayer = await responseGetLstPlayer.Content.ReadFromJsonAsync<List<PlayerDTO>>();
                ViewBag.PlayerLst = lstPlayer;
            }
            else
            {
                var status = responseGetTourdetail.StatusCode;
                return RedirectToAction("InternalServerError", "Error");
            }
            return View();
        }
    }
}
