using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
