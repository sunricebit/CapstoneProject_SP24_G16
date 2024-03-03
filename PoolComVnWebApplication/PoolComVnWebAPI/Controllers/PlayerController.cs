using AutoMapper;
using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PoolComVnWebAPI.DTO;
using System.Collections.Generic;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerDAO _playerDAO;
        private readonly IMapper _mapper;

        public PlayerController(PlayerDAO playerDAO, IMapper mapper)
        {
            _playerDAO = playerDAO;
            _mapper = mapper;
        }

        // GET: api/Player
        [HttpGet]
        public ActionResult<IEnumerable<PlayerDTO>> Get()
        {
            var players = _playerDAO.GetAllPlayers();
            var playersDto = _mapper.Map<List<PlayerDTO>>(players);
            return Ok(playersDto);
        }

        // GET: api/Player/5
        [HttpGet("{id}")]
        public ActionResult<PlayerDTO> Get(int id)
        {
            var player = _playerDAO.GetPlayerById(id);

            if (player == null)
            {
                return NotFound();
            }

            var playerDto = _mapper.Map<PlayerDTO>(player);
            return Ok(playerDto);
        }

        // POST: api/Player
        [HttpPost]
        public IActionResult Post([FromBody] PlayerDTO playerDto)
        {
            if (playerDto == null)
            {
                return BadRequest();
            }

            var player = _mapper.Map<Player>(playerDto);

            // Ensure PlayerID is not set explicitly, allowing the database to generate it
            player.PlayerID = 0;

            _playerDAO.AddPlayer(player);

            return CreatedAtAction(nameof(Get), new { id = player.PlayerID }, playerDto);
        }

        // PUT: api/Player/5
        [HttpPut("{id}")]
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

            // Use the PlayerDTO properties to update the existingPlayer
            existingPlayer.PlayerName = updatedPlayerDto.PlayerName;
            existingPlayer.AccountID = updatedPlayerDto.AccountID;
            existingPlayer.Level = updatedPlayerDto.Level;

            // Update the existing entity
            _playerDAO.UpdatePlayer(existingPlayer);

            return NoContent();
        }

        // DELETE: api/Player/5
        [HttpDelete("{id}")]
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

        // POST: api/Player/ImportExcel
        [HttpPost("ImportExcel")]
        public IActionResult ImportExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length <= 0)
                {
                    return BadRequest("Invalid file.");
                }

                // Check file extension
                var fileExtension = Path.GetExtension(file.FileName)?.ToLower();
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    // Handle Excel file
                    using (var package = new ExcelPackage(file.OpenReadStream()))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet

                        // Skip header row
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var playerName = worksheet.Cells[row, 1].Text?.Trim();
                            var accountIDText = worksheet.Cells[row, 2].Text?.Trim();
                            var level = worksheet.Cells[row, 3].Text?.Trim();

                            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(accountIDText) || string.IsNullOrEmpty(level))
                            {
                                // Log or handle missing data
                                continue;
                            }

                            if (!int.TryParse(accountIDText, out int accountID))
                            {
                                // Log or handle invalid AccountID format
                                continue;
                            }

                            var player = new Player
                            {
                                PlayerName = playerName,
                                AccountID = accountID,
                                Level = level
                            };

                            _playerDAO.AddPlayer(player);
                        }
                    }
                }
                else if (fileExtension == ".csv")
                {
                    // Handle CSV file
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        // Skip header row
                        reader.ReadLine();

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(',');

                            var playerName = values[0]?.Trim();
                            var accountIDText = values[1]?.Trim();
                            var level = values[2]?.Trim();

                            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(accountIDText) || string.IsNullOrEmpty(level))
                            {
                                // Log or handle missing data
                                continue;
                            }

                            if (!int.TryParse(accountIDText, out int accountID))
                            {
                                // Log or handle invalid AccountID format
                                continue;
                            }

                            var player = new Player
                            {
                                PlayerName = playerName,
                                AccountID = accountID,
                                Level = level
                            };

                            _playerDAO.AddPlayer(player);
                        }
                    }
                }
                else
                {
                    return BadRequest("Unsupported file format. Please upload a valid Excel (.xls, .xlsx) or CSV file.");
                }

                return Ok("Data imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
