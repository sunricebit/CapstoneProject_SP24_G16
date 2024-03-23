using BusinessObject.Models;
using DataAccess;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Text.Json;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static PoolComVnWebAPI.DTO.CreateTourStepOneDTO;

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
        //[Authorize]
        public IActionResult Index()
        {
            List<Tournament> tournaments = _tournamentDAO.GetAllTournament().ToList();
            return Ok(tournaments);
        }

        [HttpGet("GetTournament")]
        public IActionResult GetTournament(int tourId)
        {
            try
            {
                Tournament tour = _tournamentDAO.GetTournament(tourId);
                TournamentDetailDTO tournamentDetailDTO = new TournamentDetailDTO() {
                    Address = tour.Club.Address,
                    ClubName = tour.Club.ClubName,
                    TournamentId = tour.TourId,
                    TournamentName = tour.TourName,
                    Description = tour.Description,
                    StartTime = tour.StartDate ?? DateTime.Now,
                    EndTime = tour.EndDate,
                    Flyer = tour.Flyer,
                    GameType = tour.GameTypeId == Constant.Game8Ball ? Constant.String8Ball
                                    : (tour.GameTypeId == Constant.Game9Ball ? Constant.String9Ball : Constant.String10Ball),
                    Status = tour.Status,
                    TourTypeId = tour.TournamentTypeId,
                    RaceWin = GetRaceWinNumbers(tour.RaceToString),
                    RaceLose = GetRaceLoseNumbers(tour.RaceToString),
                    RegisterDate = tour.RegistrationDeadline,
                    MaxPlayer = tour.MaxPlayerNumber,
                    Access = tour.Access == null ? true : tour.Access.Value,
                    EntryFee = tour.EntryFee,
                    TotalPrize = tour.TotalPrize,
                };
                return Ok(tournamentDetailDTO);
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        private List<RaceNumber> GetRaceWinNumbers(string raceString)
        {
            List<RaceNumber> raceNumbers = new List<RaceNumber>();

            string[] parts = raceString.Split(',');

            foreach (var part in parts)
            {
                string[] subParts = part.Split('-');

                if (subParts.Length == 2 && int.TryParse(subParts[1], out int gameToWin) && subParts[0].StartsWith("W"))
                {
                    string round = "R" + subParts[0].Substring(1);
                    RaceNumber raceNumber = new RaceNumber
                    {
                        Round = subParts[0],
                        GameToWin = gameToWin
                    };

                    raceNumbers.Add(raceNumber);
                }
            }

            raceNumbers.Last().Round = "CK";
            return raceNumbers;
        }

        private List<RaceNumber> GetRaceLoseNumbers(string raceString)
        {
            List<RaceNumber> raceNumbers = new List<RaceNumber>();

            string[] parts = raceString.Split(',');

            foreach (var part in parts)
            {
                string[] subParts = part.Split('-');

                if (subParts.Length == 2 && int.TryParse(subParts[1], out int gameToWin) && subParts[0].StartsWith("L"))
                {
                    string round = "R" + subParts[0].Substring(1);
                    RaceNumber raceNumber = new RaceNumber
                    {
                        Round = subParts[0],
                        GameToWin = gameToWin
                    };

                    raceNumbers.Add(raceNumber);
                }
            }

            return raceNumbers;
        }

        [HttpPost]
        public IActionResult UpdateTournament([FromBody] TournamentDTO tournamentDTO)
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

        public IActionResult CreateTourStTwo([FromBody] CreateTourStepFourDTO BannerDTO)
        {
           
            //var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

       
            //var handler = new JwtSecurityTokenHandler();
            //var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

           
            //var roleClaim = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Role"));
            //var account = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Account"));
            ////if (!Constant.BusinessRole.ToString().Equals(roleClaim.Value))
            ////{
            ////    return BadRequest("Unauthorize");
            ////}

            //int clubId = _clubDAO.GetClubIdByAccountId(Int32.Parse(account.Value));

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


        [HttpGet("SearchTournament")]
        public ActionResult<IEnumerable<TournamentOutputDTO>> SearchTournament(string searchQuery)
        {
            try
            {
                var tournaments = _tournamentDAO.GetTournamentBySearch(searchQuery);

                if (tournaments == null || tournaments.Count == 0)
                {
                    return NotFound("Không tìm thấy giải đấu nào phù hợp.");
                }

                var tournamentDTOs = new List<TournamentOutputDTO>();
                foreach (var tour in tournaments)
                {
                    var tourDTO = new TournamentOutputDTO
                    {
                        TournamentId = tour.TourId,
                        TournamentName = tour.TourName,
                        StartTime = tour.StartDate ?? DateTime.Now,
                        EndTime = tour.EndDate,
                        Address = tour.Club.Address,
                        ClubName = tour.Club.ClubName,
                        Description = tour.Description,
                        GameType = tour.GameTypeId == Constant.Game8Ball ? Constant.String8Ball :
                                   (tour.GameTypeId == Constant.Game9Ball ? Constant.String9Ball : Constant.String10Ball),
                        Status = tour.Status,
                        Flyer = tour.Flyer
                    };
                    tournamentDTOs.Add(tourDTO);
                }
                    return Ok(tournamentDTOs);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có lỗi xảy ra
                return StatusCode(500, $"Lỗi khi tìm kiếm giải đấu: {ex.Message}");
            }
        }

        [HttpGet("FilterTournaments")]
        public IActionResult FilterTournaments(string? gameTypeName, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var tournaments = _tournamentDAO.GetTournamentsByFilters(gameTypeName, startDate, endDate);

                // Chuyển đổi danh sách giải đấu thành danh sách DTO
                List<TournamentOutputDTO> tournamentDTOs = new List<TournamentOutputDTO>();
                foreach (var tournament in tournaments)
                {
                    var tournamentDTO = new TournamentOutputDTO
                    {
                        TournamentId = tournament.TourId,
                        TournamentName = tournament.TourName,
                        StartTime = tournament.StartDate ?? DateTime.Now,
                        EndTime = tournament.EndDate,
                        Address = tournament.Club.Address,
                        ClubName = tournament.Club.ClubName,
                        Description = tournament.Description,
                        GameType = tournament.GameTypeId == Constant.Game8Ball ? Constant.String8Ball
                                    : (tournament.GameTypeId == Constant.Game9Ball ? Constant.String9Ball : Constant.String10Ball),
                        Status = tournament.Status,
                        Flyer = tournament.Flyer
                    };
                    tournamentDTOs.Add(tournamentDTO);
                }

                // Trả về danh sách giải đấu
                return Ok(tournamentDTOs);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        [HttpGet("GetAllTour")]
        public IActionResult GetAllTour()
        {
            try
            {
                List<TournamentOutputDTO> allTourDto = new List<TournamentOutputDTO>();
                var allTourLst = _tournamentDAO.GetAllTournament();
                foreach(var item in allTourLst)
                {
                    TournamentOutputDTO tour = new TournamentOutputDTO()
                    {
                        TournamentId = item.TourId,
                        TournamentName = item.TourName,
                        StartTime = item.StartDate ?? DateTime.Now,
                        EndTime = item.EndDate,
                        Address = item.Club.Address,
                        ClubName = item.Club.ClubName,
                        Description = item.Description,
                        GameType = item.GameTypeId == Constant.Game8Ball ? Constant.String8Ball 
                                    : (item.GameTypeId == Constant.Game9Ball ? Constant.String9Ball : Constant.String10Ball),
                        Status = item.Status,
                        Flyer = item.Flyer,
                    };
                    allTourDto.Add(tour);
                }
                return Ok(allTourDto);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        [HttpGet("GetTourInfo")]
        public IActionResult GetTourInfo(int tourId)
        {
            try
            {
                Tournament tour = _tournamentDAO.GetTournament(tourId);
                return Ok(new { playerNumber = tour.PlayerNumber, 
                    finalSinglePlayer = tour.KnockoutPlayerNumber});
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
