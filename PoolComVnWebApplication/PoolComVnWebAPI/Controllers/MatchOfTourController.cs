using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;
using System.Text.RegularExpressions;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchOfTourController : ControllerBase
    {
        private readonly MatchDAO _matchDAO;
        private readonly ClubDAO _clubDAO;
        private readonly PlayerDAO _playerDAO;
        private readonly TournamentDAO _tournamentDAO;

        public MatchOfTourController(MatchDAO matchDAO, ClubDAO clubDAO, PlayerDAO playerDAO, TournamentDAO tournamentDAO)
        {
            _matchDAO = matchDAO;
            _clubDAO = clubDAO;
            _playerDAO = playerDAO;
            _tournamentDAO = tournamentDAO;
        }

        [HttpGet("GetMatchForBracket")]
        public IActionResult GetMatchForBracket(int tourId)
        {
            var lstMatch = _matchDAO.GetMatchOfTournaments(tourId);
            List<MatchOfTournamentOutputDTO> lstOutputMatch = new List<MatchOfTournamentOutputDTO>();
            foreach (var match in lstMatch)
            {
                var players = _playerDAO.GetPlayersByMatchTour(match.MatchId);
                var p1 = players.OrderBy(p => p.PlayerMatchId).FirstOrDefault();
                var p2 = players.OrderBy(p => p.PlayerMatchId).LastOrDefault();

                MatchOfTournamentOutputDTO matchOfTournament = new MatchOfTournamentOutputDTO()
                {
                    MatchNumber = match.MatchNumber,
                    MatchCode = match.MatchCode,
                    LoseNextMatch = match.LoseToMatch,
                    WinNextMatch = match.WinToMatch,
                    P1Id = p1 != null ? p1.PlayerId : null,
                    P2Id = p2 != null ? p2.PlayerId : null,
                    P1Country = p1 != null ? p1.Player.Country.CountryImage : null,
                    P1Name = p1 != null ? p1.Player.PlayerName : null,
                    P1Score = p1 == null ? "_" : p1.Score.ToString(),
                    P2Country = p2 != null ? p2.Player.Country.CountryImage : null,
                    P2Name =  p2 != null ? p2.Player.PlayerName : null,
                    P2Score = p2 == null ? "_" : p2.Score.ToString(),
                };

                lstOutputMatch.Add(matchOfTournament);
            }
            return Ok(lstOutputMatch);
        }

        [HttpGet("RandomMatch")]
        public IActionResult RandomMatch(int tourId)
        {
            var playersInTour = _playerDAO.GetPlayersByTournament(tourId).ToList();
            var botPlayer = playersInTour.Where(p => p.PlayerName.Equals("BOT")).ToList();

            foreach (var player in botPlayer)
            {
                playersInTour.Remove(player);
            }

            List<MatchOfTournamentOutputDTO> lstMatch = new List<MatchOfTournamentOutputDTO>();
            int count = playersInTour.Count() + botPlayer.Count();
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

                if (botPlayer.Count() > 0)
                {
                    Player p2 = botPlayer[0];
                    matchOfTournament.P2Name = p2.PlayerName;
                    matchOfTournament.P2Id = p2.PlayerId;
                    matchOfTournament.P2Country = p2.Country.CountryImage;
                    botPlayer.Remove(p2);
                    lstMatch.Add(matchOfTournament);
                }
                else
                {
                    randomId = rand.Next(0, playersInTour.Count - 1);
                    Player p2 = playersInTour[randomId];
                    matchOfTournament.P2Name = p2.PlayerName;
                    matchOfTournament.P2Id = p2.PlayerId;
                    matchOfTournament.P2Country = p2.Country.CountryImage;
                    playersInTour.Remove(p2);
                    lstMatch.Add(matchOfTournament);
                }
            }


            return Ok(lstMatch);
        }

        [HttpPost("SaveMatchesRandom/{tourId}")]
        public IActionResult SaveMatchesRandom(int tourId, [FromBody] List<MatchOfTournamentOutputDTO> lstMatch)
        {
            //var lstMatch = _matchDAO.GetMatchOfTournaments(tourId);
            int count = 1;
            foreach (var match in lstMatch)
            {
                MatchOfTournament matchOfTournament = new MatchOfTournament()
                {
                    MatchCode = "W1-" + match.MatchNumber,
                    TourId = tourId,
                    MatchNumber = count,
                    Status = 0,
                    StartTime = DateTime.Now,
                };
                count++;
                _matchDAO.AddMatch(matchOfTournament);
                _playerDAO.AddPlayerToMatch(matchOfTournament.MatchId, match.P1Id.Value);
                _playerDAO.AddPlayerToMatch(matchOfTournament.MatchId, match.P2Id.Value);
            }
            GenerateAllMatchOfTour(tourId);
            return Ok();
        }

        private void GenerateAllMatchOfTour(int tourId)
        {
            Tournament tour = _tournamentDAO.GetTournament(tourId);
            int numberOfMatch = CalculateNumberOfMatch(tour.MaxPlayerNumber, tour.KnockoutPlayerNumber);
            for (int i = 1; i <= numberOfMatch; i++)
            {
                if (!_matchDAO.CheckExistMatch(tourId, i))
                {
                    MatchOfTournament matchOfTournament = new MatchOfTournament()
                    {
                        TourId = tourId,
                        MatchCode = i.ToString(),
                        MatchNumber = i,
                        WinToMatch = WinNextMatch(i, tour.MaxPlayerNumber, tour.KnockoutPlayerNumber.Value),
                        LoseToMatch = LoseNextMatch(i, tour.MaxPlayerNumber, tour.KnockoutPlayerNumber.Value),
                        Status = 0,
                    };
                    _matchDAO.AddMatch(matchOfTournament);
                }
                else
                {
                    MatchOfTournament matchOfTournament = _matchDAO.GetMatchOfTournamentsByNumber(tourId, i);
                    matchOfTournament.LoseToMatch = LoseNextMatch(i, tour.MaxPlayerNumber, tour.KnockoutPlayerNumber.Value);
                    matchOfTournament.WinToMatch = WinNextMatch(i, tour.MaxPlayerNumber, tour.KnockoutPlayerNumber.Value);
                    _matchDAO.UpdateMatch(matchOfTournament);
                }
            }
        }

        private int CalculateNumberOfMatch(int numberOfPlayer, int? knockOutPlayer)
        {
            if (knockOutPlayer != null)
            {
                int count = (int)Math.Log2(numberOfPlayer / knockOutPlayer.Value);
                int numberOfMatch = numberOfPlayer / 2;
                for (int i = 1; i <= count; i++)
                {
                    numberOfMatch += (int)(numberOfPlayer / Math.Pow(2, i));
                    numberOfMatch += (int)(numberOfPlayer / Math.Pow(2, i + 1));
                }
                count = (int)Math.Log2(knockOutPlayer.Value);
                for (int i = 1; i <= count; i++)
                {
                    numberOfMatch += (int)(knockOutPlayer / Math.Pow(2, i));
                }
                return numberOfMatch;
            }
            else{
                int count = (int)Math.Log2(numberOfPlayer);
                int numberOfMatch = numberOfPlayer / 2;
                for (int i = 1; i <= count; i++)
                {
                    numberOfMatch += (int)(numberOfPlayer / Math.Pow(2, i));
                }
                return numberOfMatch;
            }
        }

        private int WinNextMatch(int m, int n, int d)
        {
            int a = Convert.ToInt32(Math.Log2(n));
            int w = Convert.ToInt32(Math.Log2(d));
            int x = 0;

            for (int i = (a - 1); i >= w; i--)
            {
                x = x + Convert.ToInt32(Math.Pow(2, i));
            }

            int y = 2 * x;
            if (m % 2 == 1 && m <= x)
            {
                return (n / 2 + (m + 1) / 2);
            }
            else if (m % 2 == 0 && m <= x)
            {
                return n / 2 + m / 2;
            }
            else if (m > x && m <= (x + Convert.ToInt32(Math.Pow(2, (w - 1)))))
            {
                return m * 2 + d / 2 - (m - x);
            }
            else if (m % 2 == 1 && m > (y + Convert.ToInt32(Math.Pow(2, (w - 1)))))
            {
                return Convert.ToInt32(0.5 * y + 0.75 * d + (m + 1) / 2);
            }
            else if (m % 2 == 0 && m > (y + Convert.ToInt32(Math.Pow(2, (w - 1)))))
            {
                return Convert.ToInt32(0.5 * y + 0.75 * d + m / 2);
            }
            else if (m > y && m <= (y + d / 2))
            {
                if (m > (y + d / 4))
                {
                    if (w == 1)
                    {
                        return m + d / 4 + 1;
                    }
                    return m + d / 4;
                }
                else
                {
                    return m + d - d / 4;
                }
            }
            else
            {
                int from = x + d / 2 + 1;
                int to = x + d / 2 + Convert.ToInt32(Math.Pow(2, a - 2));
                for (int i = (a - 2); i >= (w - 1); i--)
                {
                    if (m >= from && m <= to)
                    {
                        return m + Convert.ToInt32(Math.Pow(2, i));
                    }
                    else if (m > to && m <= (to + Convert.ToInt32(Math.Pow(2, i))))
                    {
                        if ((m % 2 == 0 && d == 2) || m % 2 == 1 && d != 2)
                        {
                            return Convert.ToInt32((m + 1 - to) / 2 + to + Math.Pow(2, i));
                        }
                        else
                        {
                            return Convert.ToInt32((m - to) / 2 + to + Math.Pow(2, i));
                        }
                    }
                    from += Convert.ToInt32(2 * Math.Pow(2, i));
                    to += Convert.ToInt32(1.5 * Math.Pow(2, i));
                }
            }
            return 0;
        }

        private int LoseNextMatch(int m, int n, int d)
        {
            int a = Convert.ToInt32(Math.Log2(n));
            int w = Convert.ToInt32(Math.Log2(d));
            int x = 0;

            for (int i = (a - 1); i >= w; i--)
            {
                x = x + Convert.ToInt32(Math.Pow(2, i));
            }

            int y = 2 * x;
            if (m % 2 == 1 && m <= n / 2)
            {
                return ((x + d / 2) + (m + 1) / 2);
            }
            else if (m % 2 == 0 && m <= n / 2)
            {
                return (x + d / 2) + m / 2;
            }
            else if (m > n / 2 && m <= 0.75 * n)
            {
                return (x + d / 2 + n + (1 - m));
            }
            else if (m > 0.75 * n && m <= x)
            {
                for (int i = a - 3; i >= w; i--)
                {
                    int count1 = 0;
                    for (int j = a - 1; j >= i + 1; j--)
                    {
                        count1 += Convert.ToInt32(Math.Pow(2, j));
                    }

                    int count2 = count1 + Convert.ToInt32(Math.Pow(2, i));

                    if (m > count1 && m <= count2)
                    {
                        if (m % 2 == 1)
                        {
                            return x + d / 2 + count1 - (count2 - m) + 1;
                        }
                        else if (m % 2 == 0)
                        {
                            return x + d / 2 + count1 - (count2 - m) - 1;
                        }
                    }
                }

            }
            else if (m % 2 == 1 && m > x && m <= (x + Convert.ToInt32(Math.Pow(2, (w - 1)))))
            {
                if (w == 1)
                {
                    return y + m - x;
                }
                return y + (m - x + 1);
            }
            else if (m % 2 == 0 && m > x && m <= (x + Convert.ToInt32(Math.Pow(2, (w - 1)))))
            {
                return y + (m - x - 1);
            }
            return 0;
        }
    }
}
