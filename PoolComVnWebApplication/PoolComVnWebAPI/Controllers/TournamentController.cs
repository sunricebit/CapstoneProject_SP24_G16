﻿using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;
using System.IdentityModel.Tokens.Jwt;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly TournamentDAO _tournamentDAO;
        private readonly ClubDAO _clubDAO;

        public TournamentController(TournamentDAO tournamentDAO, ClubDAO clubDAO)
        {
            _tournamentDAO = tournamentDAO;
            _clubDAO = clubDAO;
        }

        [HttpGet("GetAllTournament")]
        [Authorize]
        public IActionResult Index()
        {
            List<Tournament> tournaments = _tournamentDAO.GetAllTournament().ToList();
            return Ok(tournaments);
        }

        [HttpGet("{tourId}")]
        public IActionResult ViewTournament(int tourId)
        {
            try
            {
                Tournament p = _tournamentDAO.GetTournament(tourId);

                return Ok(p);
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        [HttpPost("{tourId}")]
        public IActionResult UpdateTournament()
        {
            return Ok();
        }

        [HttpPost("CreateTourStOne")]
        [Authorize]
        public IActionResult CreateTourStOne([FromBody] CreateTourStepOneDTO inputDto)
        {
            // Lấy giá trị token từ header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Giải mã token để lấy các claims
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Xử lý logic của bạn với các claims
            var roleClaim = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Role"));
            var account = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Account"));
            if (!Constant.BusinessRole.ToString().Equals(roleClaim.Value))
            {
                return BadRequest("Unauthorize");
            }

            int clubId = _clubDAO.GetClubIdByAccountId(Int32.Parse(account.Value));

            try
            {
                Tournament tour = new Tournament()
                {
                    TourName = inputDto.TournamentName,
                    Accesses = inputDto.Access,
                    ClubId = clubId,
                    Description = inputDto.Description,
                    StartDate = inputDto.StartTime,
                    EndDate = inputDto.EndTime,
                    EntryFee = inputDto.EntryFee.Value,
                    KnockoutPlayerNumber = inputDto.TournamentTypeId == Constant.DoubleEliminate ? inputDto.KnockoutNumber : null,
                    GameTypeId = inputDto.GameTypeId,
                    TotalPrize = inputDto.PrizeMoney,
                    TournamentTypeId = inputDto.TournamentTypeId,
                    MaxPlayerNumber = inputDto.MaxPlayerNumber,
                    RegistrationDeadline = inputDto.RegistrationDeadline,
                    RaceToString = inputDto.RaceNumberString,
                };
                _tournamentDAO.CreateTournament(tour);
                return Ok("Create tournament Successful");
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
