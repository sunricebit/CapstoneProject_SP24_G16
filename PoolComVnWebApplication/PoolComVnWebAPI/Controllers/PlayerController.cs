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
        private readonly IMapper _mapper;
        private static List<PlayerDTO> selectedPlayers = new List<PlayerDTO>();
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

            // Modify the PhoneNumber property using foreach loop
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
                PhoneNumber = player.PhoneNumber, // Use the modified PhoneNumber property
                Level = player.Level,
            }));
        }


        // GET: api/Player/GetByName/{name}
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

        // POST: api/Player
        [HttpGet("AddPlayer")]
        public IActionResult Post([FromBody] PlayerDTO playerDto)
        {
            if (playerDto == null)
            {
                return BadRequest();
            }

            var player = _mapper.Map<Player>(playerDto);

            // Ensure PlayerID is not set explicitly, allowing the database to generate it
            player.PlayerId = 0;

            _playerDAO.AddPlayer(player);

            var createdPlayerDto = _mapper.Map<PlayerDTO>(player);
            return CreatedAtAction(nameof(Get), new { id = createdPlayerDto.PlayerId }, createdPlayerDto);
        }

        // PUT: api/Player/5
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

            // Update the existingPlayer with properties from updatedPlayerDto
            existingPlayer.PlayerName = updatedPlayerDto.PlayerName;
            existingPlayer.Level = updatedPlayerDto.Level.Value;

            // Assuming User and Account are not null
            existingPlayer.User.Account.PhoneNumber = updatedPlayerDto.PhoneNumber;

            // Update the existing entity
            _playerDAO.UpdatePlayer(existingPlayer);

            return NoContent();
        }

        // DELETE: api/Player/5
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

        // POST: api/Player/ImportExcel
        [HttpPost("ImportExcel")]
        public IActionResult ImportPlayers(IFormFile file)
        {

            try
            {
                if (file == null || file.Length <= 0)
                {
                    return BadRequest("Invalid file.");
                }

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
                            var countryName = worksheet.Cells[row, 2].Text?.Trim();
                            var phoneNumber = worksheet.Cells[row, 3].Text?.Trim();
                            var levelText = worksheet.Cells[row, 4].Text?.Trim();

                            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(countryName) ||
                                string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(levelText))
                            {
                                // Log or handle missing data
                                continue;
                            }

                            if (!int.TryParse(levelText, out int level))
                            {
                                // Log or handle invalid level format
                                continue;
                            }

                            var player = new PlayerDTO
                            {
                                PlayerName = playerName,
                                CountryName = countryName,
                                PhoneNumber = phoneNumber,
                                Level = level
                            };

                            //_playerDAO.AddPlayersFromExcel(new List<PlayerDTO> { player });
                        }
                    }
                }
                else
                {
                    return BadRequest("Unsupported file format. Please upload a valid Excel (.xls, .xlsx) file.");
                }

                return Ok("Data imported successfully.");
            }
            catch (IOException ex)
            {
                // Log or handle file-related exceptions
                return BadRequest($"Error reading the file: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // GET: api/Player/DownloadTemplate
        [HttpGet("DownloadTemplate")]
        public IActionResult DownloadTemplate()
        {
            try
            {
                // Thiết lập LicenseContext cho EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Tạo một MemoryStream để lưu trữ dữ liệu Excel
                using (var stream = new MemoryStream())
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        // Tạo một worksheet và thiết lập các tiêu đề cột
                        var worksheet = package.Workbook.Worksheets.Add("PlayerTemplate");

                        // Thiết lập màu nền và màu chữ cho hàng tiêu đề
                        var headerCells = worksheet.Cells["A1:D1"];
                        headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 128, 0)); // Màu xanh
                        headerCells.Style.Font.Color.SetColor(System.Drawing.Color.White); // Màu chữ trắng

                        // Thiết lập các tiêu đề cột
                        worksheet.Cells["A1"].Value = "Tên";
                        worksheet.Cells["B1"].Value = "Quốc Gia";
                        worksheet.Cells["C1"].Value = "Số Điện Thoại";
                        worksheet.Cells["D1"].Value = "Hạng";

                        // Thêm dòng ví dụ
                        worksheet.Cells["A2"].Value = "Nguyễn Văn A";
                        worksheet.Cells["B2"].Value = "Việt Nam";
                        worksheet.Cells["C2"].Value = "123456789";
                        worksheet.Cells["D2"].Value = 1;

                        worksheet.Cells["A3"].Value = "Alex josh";
                        worksheet.Cells["B3"].Value = "Russia";
                        worksheet.Cells["C3"].Value = "987654321";
                        worksheet.Cells["D3"].Value = 2;

                        // Lưu trữ workbook và trả về dữ liệu như là một File Content Result
                        package.Save(); // Lưu trước khi đọc từ MemoryStream
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PlayerTemplate.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về lỗi nếu cần
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
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
                };
                lstPlayer.Add(playerDTO);
            }
            return Ok(players);
        }   
    }
}
