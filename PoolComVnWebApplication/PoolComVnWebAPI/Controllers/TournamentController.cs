using BusinessObject.Models;
using DataAccess;
using Firebase.Auth;
using Firebase.Storage;
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
        
        public TournamentController(TournamentDAO tournamentDAO)
        {
            _tournamentDAO = tournamentDAO;
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
