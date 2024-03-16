using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchOfTourController : ControllerBase
    {
        private readonly MatchDAO _matchDAO;
        private readonly ClubDAO _clubDAO;

        public MatchOfTourController(MatchDAO matchDAO, ClubDAO clubDAO)
        {
            _matchDAO = matchDAO;
            _clubDAO = clubDAO;
        }

        [HttpGet]
        public IActionResult GetMatchForBracket(int tourId)
        {
            var lstMatch = _matchDAO.GetMatchOfTournaments(tourId);
            foreach (var match in lstMatch)
            {
                MatchOfTournamentOutputDTO matchOfTournament = new MatchOfTournamentOutputDTO();

            }
            return Ok();
        }

        [HttpGet]
        public IActionResult RandomMatch(int tourId)
        {
            var lstMatch = _matchDAO.GetMatchOfTournaments(tourId);
            foreach (var match in lstMatch)
            {
                MatchOfTournamentOutputDTO matchOfTournament = new MatchOfTournamentOutputDTO();

            }
            return Ok();
        }

        [HttpPost]
        public IActionResult SaveMatchesRandom([FromBody] List<MatchOfTournamentOutputDTO> lstMatch)
        {
            //var lstMatch = _matchDAO.GetMatchOfTournaments(tourId);
            foreach (var match in lstMatch)
            {
                MatchOfTournamentOutputDTO matchOfTournament = new MatchOfTournamentOutputDTO();

            }
            return Ok();
        }


    }
}
