using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;
using System.IdentityModel.Tokens.Jwt;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateTournamentController : ControllerBase
    {
        private readonly ClubDAO _clubDAO;
        private readonly TournamentDAO _tournamentDAO;

        public CreateTournamentController(ClubDAO clubDAO, TournamentDAO tournamentDAO)
        {
            _clubDAO = clubDAO;
            _tournamentDAO = tournamentDAO;
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
            var club = _clubDAO.GetClubByAccountId(Int32.Parse(account.Value));
            int clubId = club.ClubId;

            try
            {
                Tournament tour = new Tournament()
                {
                    TourName = inputDto.TournamentName,
                    Access = inputDto.Access,
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
                    Status = Constant.TournamentIncoming,
                };
                _tournamentDAO.CreateTournament(tour);
                return Ok(_tournamentDAO.GetLastestTournament().TourId);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        [HttpPost("CreateTourStFour")]
        // [Authorize]

        public IActionResult CreateTourStTwo([FromBody] CreateTourStepTwoDTO BannerDTO)
        {
            try
            {
                Tournament tour = _tournamentDAO.GetTournament(BannerDTO.TourID);
                tour.Flyer = BannerDTO.Flyer;
                _tournamentDAO.UpdateTournament(tour);
                return Ok(BannerDTO.TourID);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

    }
}
