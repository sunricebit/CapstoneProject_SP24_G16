using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;

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

        [HttpGet("GetTournamentForStOne")]
        public IActionResult GetTournamentForStOne(int tourId)
        {
            try
            {
                Tournament tour = _tournamentDAO.GetTournament(tourId);
                TournamentDTO tournamentDetailDTO = new TournamentDTO()
                {
                    TourId = tour.TourId,
                    TourName = tour.TourName,
                    Description = tour.Description,
                    StartDate = tour.StartDate ?? DateTime.Now,
                    EndDate = tour.EndDate,
                    GameTypeId = tour.GameTypeId,
                    Status = tour.Status,
                    TournamentTypeId = tour.TournamentTypeId,
                    RegistrationDeadline = tour.RegistrationDeadline,
                    MaxPlayerNumber = tour.MaxPlayerNumber,
                    Access = tour.Access == null ? true : tour.Access.Value,
                    EntryFee = tour.EntryFee,
                    TotalPrize = tour.TotalPrize,
                    KnockoutPlayerNumber = tour.KnockoutPlayerNumber,
                };
                return Ok(tournamentDetailDTO);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet("GetFlyer")]
        public IActionResult GetFlyer(int tourId)
        {
            try
            {
                Tournament tour = _tournamentDAO.GetTournament(tourId);
                return Ok(tour.Flyer);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet("GetTournamentsByClubId")]
        public IActionResult GetTournamentsByClubId(int clubId)
        {
            try
            {
                List<Tournament> tournaments = _tournamentDAO.GetTournamentsByClubId(clubId);
                List<TournamentDetailDTO> tournamentDetails = new List<TournamentDetailDTO>();
                foreach (var tour in tournaments)
                {
                    TournamentDetailDTO tournamentDetailDTO = new TournamentDetailDTO()
                    {
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

                    tournamentDetails.Add(tournamentDetailDTO);
                }
                return Ok(tournamentDetails);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Lỗi khi lấy thông tin giải đấu: {e.Message}");
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
            var tournament = _tournamentDAO.GetTournament(tournamentDTO.TourId);
            tournament.TourName = tournamentDTO.TourName;
            tournament.MaxPlayerNumber = tournamentDTO.MaxPlayerNumber;
            tournament.Description = tournamentDTO.Description;
            tournament.StartDate = tournamentDTO.StartDate;
            tournament.EndDate = tournamentDTO.EndDate;
            tournament.GameTypeId = tournamentDTO.GameTypeId;
            tournament.TournamentTypeId = tournamentDTO.TournamentTypeId;
            tournament.KnockoutPlayerNumber = tournamentDTO.KnockoutPlayerNumber;
            tournament.RaceToString = tournamentDTO.RaceToString;
            tournament.EntryFee = tournament.EntryFee;
            tournament.TotalPrize = tournament.TotalPrize;
            tournament.RegistrationDeadline = tournament.RegistrationDeadline;
            tournament.Access = tournament.Access;
            _tournamentDAO.UpdateTournament(tournament);
            return Ok();
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

        [HttpGet("GetTourMaxNumberOfPlayer")]
        public IActionResult GetTourMaxNumberOfPlayer(int tourId)
        {
            try
            {
                int maxNumberPlayer = _tournamentDAO.GetTourMaxNumberOfPlayer(tourId);
                return Ok(maxNumberPlayer);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet("GetTourKnockoutNumber")]
        public IActionResult GetTourKnockoutNumber(int tourId)
        {
            try
            {
                int? knockOutNumber = _tournamentDAO.GetTourKnockoutNumber(tourId);
                return Ok(knockOutNumber);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
