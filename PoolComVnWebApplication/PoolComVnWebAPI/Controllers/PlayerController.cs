using AutoMapper;
using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PoolComVnWebAPI.DTO;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerDAO _playerDAO;
        private readonly TournamentDAO _tournamentDAO;
        private readonly AccountDAO _accountDAO;
        private readonly IMapper _mapper;
        private static List<PlayerDTO> selectedPlayers = new List<PlayerDTO>();
        public PlayerController(PlayerDAO playerDAO,  IMapper mapper, TournamentDAO tournamentDAO, AccountDAO accountDAO)
        {
            _playerDAO = playerDAO;
            _mapper = mapper;
            _tournamentDAO = tournamentDAO;
            _accountDAO = accountDAO;
        }


        [HttpGet]
        public ActionResult<IEnumerable<PlayerDTO>> Get()
        {
            var players = _playerDAO.GetAllPlayers();


            foreach (var playerDto in players)
            {
                if (playerDto.User != null && playerDto.User.Account != null)
                {
                    playerDto.PhoneNumber = playerDto.User.Account.PhoneNumber;
                }
            }

            return Ok(players.Select(player => new PlayerDTO
            {
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName,
                CountryName = player.Country?.CountryName,
                PhoneNumber = player.PhoneNumber,
                Level = player.Level,
            }));
        }



        [HttpGet("GetByName/{name}")]
        public ActionResult<PlayerDTO> GetByName(string name)
        {
            var player = _playerDAO.GetPlayerByName(name);

            if (player == null)
            {
                return NotFound();
            }

            var playerDto = _mapper.Map<PlayerDTO>(player);
            return Ok(playerDto);
        }


        [HttpPost("AddPlayer")]
        public IActionResult AddPlayer([FromBody] PlayerDTO playerDto)
        {
            try
            {
                var account = _accountDAO.GetAccountByEmail(playerDto.Email);
                var user = _accountDAO.GetUserByAccountById(account.AccountId);
                var tour = _tournamentDAO.GetTournament(playerDto.TourId??0);
                var country = _playerDAO.GetCountryByID(playerDto.CountryId);
                if (playerDto == null)
                {
                    return BadRequest("Dữ liệu người chơi không hợp lệ.");
                }

                var player = new Player
                {
                    PlayerName = playerDto.PlayerName,
                    CountryId = playerDto.CountryId,
                    Level = playerDto.Level ?? 0,
                    UserId =user.UserId,
                    TourId = playerDto.TourId,
                    PhoneNumber = playerDto.PhoneNumber,
                    Email = playerDto.Email,
                    IsPayed = playerDto.IsPayed,
                    User = user,
                    Tour = tour,
                    Country = country

                };

                _playerDAO.AddPlayer(player);

                return Ok();
            }
            catch (Exception ex)
            {
              
                return StatusCode(500, $"Đã xảy ra lỗi khi thêm người chơi: {ex.Message}");
            }
        }
        [HttpPost("AddPlayerToSoloMatch")]
        public IActionResult AddPlayerToSoloMatch([FromBody] PlayerInSoloMatchDTO playerInSoloMatchDTO)
        {
            try
            {
                var playerInSoloMatch = new PlayerInSoloMatch
                {
                    SoloMatchId = playerInSoloMatchDTO.SoloMatchId,
                    PlayerId = playerInSoloMatchDTO.PlayerId,
                    Score = playerInSoloMatchDTO.Score,
                    GameWin = playerInSoloMatchDTO.GameWin,
                    IsWinner = playerInSoloMatchDTO.IsWinner
                };
                _playerDAO.AddPlayerToSoloMatch(playerInSoloMatch);
                return Ok("Người chơi đã được thêm vào trận đấu đơn thành công.");
            }
            catch (Exception e)
            {

                return StatusCode(500, $"Lỗi server: {e.Message}");
            }
        }

        [HttpPost("AddPlayerToTournament")]
        public IActionResult AddPlayerToTournament([FromBody] List<PlayerDTO> playerDtos)
        {
            try
            {

                if (playerDtos == null || playerDtos.Count == 0)
                {
                    return BadRequest("No players provided.");
                }


                foreach (var player in playerDtos)
                {
                    var player2 = _mapper.Map<Player>(player);
                    player2.PlayerId = 0;
                    player2.CountryId = Constant.NationVietNamId;
                    player2.TourId = player.TourId;
                    _playerDAO.AddPlayer(player2);
                }


                return Ok("Players added to the tournament successfully.");
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("Update/{id}")]
        public IActionResult Put(int id, [FromBody] PlayerDTO updatedPlayerDto)
        {
            if (updatedPlayerDto == null)
            {
                return BadRequest("Invalid request data");
            }

            var existingPlayer = _playerDAO.GetPlayerById(id);

            if (existingPlayer == null)
            {
                return NotFound();
            }


            existingPlayer.PlayerName = updatedPlayerDto.PlayerName;
            existingPlayer.Level = updatedPlayerDto.Level.Value;


            existingPlayer.User.Account.PhoneNumber = updatedPlayerDto.PhoneNumber;


            _playerDAO.UpdatePlayer(existingPlayer);

            return NoContent();
        }

        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var player = _playerDAO.GetPlayerById(id);

            if (player == null)
            {
                return NotFound();
            }

            _playerDAO.DeletePlayer(id);

            return NoContent();
        }

        [HttpGet("GetPlayerByTourId")]
        public IActionResult GetPlayerByTourId(int tourId)
        {
            var players = _playerDAO.GetPlayersByTournament(tourId);
            List<PlayerDTO> lstPlayer = new List<PlayerDTO>();
            foreach (Player p in players)
            {
                PlayerDTO playerDTO = new PlayerDTO
                {
                    PlayerId = p.PlayerId,
                    PlayerName = p.PlayerName,
                    CountryName = p.Country.CountryImage,
                };
                lstPlayer.Add(playerDTO);
            }
            return Ok(lstPlayer);
        }

        [HttpGet("GetPlayerExBotByTourId")]
        public IActionResult GetPlayerExBotByTourId(int tourId)
        {
            var players = _playerDAO.GetPlayersByTournament(tourId);
            List<PlayerDTO> lstPlayer = new List<PlayerDTO>();
            foreach (Player p in players)
            {
                PlayerDTO playerDTO = new PlayerDTO
                {
                    PlayerId = p.PlayerId,
                    PlayerName = p.PlayerName,
                    CountryName = p.Country.CountryImage,
                };
                if (!playerDTO.PlayerName.Equals("BOT"))
                {
                    lstPlayer.Add(playerDTO);
                }
            }
            return Ok(lstPlayer);
        }

        [HttpGet("GetNumberPlayerByTourId")]
        public IActionResult GetNumberPlayerByTourId(int tourId)
        {
            int numberPlayers = _playerDAO.GetNumberPlayerByTourId(tourId);
            return Ok(numberPlayers);
        }

        [HttpGet("GenerateBotInTour")]
        public IActionResult GenerateBotInTour(int tourId)
        {
            int numberHumanPlayers = _playerDAO.GetNumberPlayerByTourId(tourId);
            int numberPlayersOfTour = _tournamentDAO.GetTournament(tourId).MaxPlayerNumber;
            int numberBotPlayer = numberPlayersOfTour - numberHumanPlayers;

            for (int i = 0; i < numberBotPlayer; i++)
            {
                Player botPlayer = new Player()
                {
                    PlayerName = "BOT",
                    CountryId = Constant.NationVietNamId,
                    TourId = tourId,
                };
                _playerDAO.AddPlayer(botPlayer);
            }

            return Ok();
        }

        [HttpGet("GetNumberPlayerExBotByTourId")]
        public IActionResult GetNumberPlayerExBotByTourId(int tourId)
        {
            int numberPlayers = _playerDAO.GetNumberPlayerExBotByTourId(tourId);
            return Ok(numberPlayers);
        }
    }
}
