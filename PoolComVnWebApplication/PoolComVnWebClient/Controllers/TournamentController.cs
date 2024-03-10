﻿using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebClient.DTO;
using PoolComVnWebClient.Common;
using System.Collections.Generic;
using System.Net.Http.Headers;

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
        public async Task<IActionResult> TournamentDetail(int tourId)
        {
            TournamentDetailDTO tourDetail;
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
            if (responseGetTourdetail.IsSuccessStatusCode)
            {
                lstPlayer = await responseGetTourdetail.Content.ReadFromJsonAsync<List<PlayerDTO>>();
                ViewBag.PlayerLst = lstPlayer;
            }
            else
            {
                var status = responseGetTourdetail.StatusCode;
                return RedirectToAction("InternalServerError", "Error");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TournamentList()
        {
            var response = await client.GetAsync(ApiUrl + "/GetAllTour");
            if (response.IsSuccessStatusCode)
            {
                List<TournamentOutputDTO> lstTour = await response.Content.ReadFromJsonAsync<List<TournamentOutputDTO>>();
                return View(lstTour);
            }
            else
            {
                var status = response.StatusCode;
            }

            return RedirectToAction("InternalServerError", "Error");
        }

        public IActionResult TournamentBracket()
        {

            return View();
        }

        public IActionResult TournamentMatchList()
        {
            return View();
        }

        public IActionResult TournamentUpcomingMatchList()
        {
            return View();
        }

        public IActionResult TournamentPlayers()
        {
            return View();
        }

        //Tournament detail for club and manage of club
        public IActionResult TournamentDetailForManager()
        {
            return View();
        }

        public IActionResult TournamentBracketForManager() {
            return View();
        }
        //[HttpPost("/Create")]
        //public
    }
}
