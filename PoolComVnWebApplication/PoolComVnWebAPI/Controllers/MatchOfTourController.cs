using BusinessObject.Models;
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
        private readonly PlayerDAO _playerDAO;

        public MatchOfTourController(MatchDAO matchDAO, ClubDAO clubDAO, PlayerDAO playerDAO)
        {
            _matchDAO = matchDAO;
            _clubDAO = clubDAO;
            _playerDAO = playerDAO;
        }

        [HttpGet("GetMatchForBracket")]
        public IActionResult GetMatchForBracket(int tourId)
        {
            var lstMatch = _matchDAO.GetMatchOfTournaments(tourId);
            List<MatchOfTournamentOutputDTO> lstOutputMatch = new List<MatchOfTournamentOutputDTO>();
            foreach (var match in lstMatch)
            {
                var players = _playerDAO.GetPlayersByMatchTour(match.MatchId);
                var p1 = players.OrderBy(p => p.PlayerMatchId).First();
                var p2 = players.OrderBy(p => p.PlayerMatchId).Last();

                MatchOfTournamentOutputDTO matchOfTournament = new MatchOfTournamentOutputDTO()
                {
                    MatchNumber = match.MatchNumber,
                    MatchCode = match.MatchCode,
                    LoseNextMatch = match.LoseToMatch,
                    WinNextMatch = match.LoseToMatch,
                    P1Country = p1.Player.Country.CountryImage,
                    P1Name = p1.Player.PlayerName,
                    P1Score = p1.Score == null ? "_" : p2.Score.ToString(),
                    P2Country = p2.Player.Country.CountryImage,
                    P2Name = p2.Player.PlayerName,
                    P2Score = p2.Score == null ? "_" : p2.Score.ToString(),
                };

                lstOutputMatch.Add(matchOfTournament);
            }
            return Ok(lstOutputMatch);
        }

        [HttpGet("RandomMatch")]
        public IActionResult RandomMatch(int tourId)
        {
            var playersInTour = _playerDAO.GetPlayersByTournament(tourId).ToList();

            List<MatchOfTournamentOutputDTO> lstMatch = new List<MatchOfTournamentOutputDTO>();
            int count = playersInTour.Count();
            for(int i = 0; i < (count/2); i++)
            {
                MatchOfTournamentOutputDTO matchOfTournament = new MatchOfTournamentOutputDTO();
                matchOfTournament.MatchNumber = i + 1;
                Random rand = new Random();
                int randomId = rand.Next(0, playersInTour.Count - 1);

                Player p1 = playersInTour[randomId];
                matchOfTournament.P1Name = p1.PlayerName;
                matchOfTournament.P1Id = p1.PlayerId;
                matchOfTournament.P1Country = p1.Country.CountryImage;
                playersInTour.Remove(p1);

                Player p2 = playersInTour[randomId];
                matchOfTournament.P2Name = p2.PlayerName;
                matchOfTournament.P2Id = p2.PlayerId;
                matchOfTournament.P2Country = p2.Country.CountryImage;
                playersInTour.Remove(p2);
                lstMatch.Add(matchOfTournament);
            }


            return Ok(lstMatch);
        }

        [HttpPost("SaveMatchesRandom")]
        public IActionResult SaveMatchesRandom([FromBody] List<MatchOfTournamentOutputDTO> lstMatch)
        {
            //var lstMatch = _matchDAO.GetMatchOfTournaments(tourId);
            foreach (var match in lstMatch)
            {
                MatchOfTournament matchOfTournament = new MatchOfTournament()
                {
                    MatchCode = match.MatchCode,
                    TourId = 1,
                    TableId = 1,
                    EndTime = DateTime.Now,
                    MatchNumber = 1,
                    Status = 0,
                    StartTime = DateTime.Now,
                };
                _matchDAO.AddMatch(matchOfTournament);

            }
            return Ok();
        }

        private void GenerateAllMatchOfTour(int tourId)
        {

        }
    }
}
