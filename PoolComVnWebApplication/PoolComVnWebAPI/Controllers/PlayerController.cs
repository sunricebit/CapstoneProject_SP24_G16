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

        
        [HttpGet("AddPlayer")]
        public IActionResult Post([FromBody] PlayerDTO playerDto)
        {
            if (playerDto == null)
            {
                return BadRequest();
            }

            var player = _mapper.Map<Player>(playerDto);

           
            player.PlayerId = 0;

            _playerDAO.AddPlayer(player);

            var createdPlayerDto = _mapper.Map<PlayerDTO>(player);
            return CreatedAtAction(nameof(Get), new { id = createdPlayerDto.PlayerId }, createdPlayerDto);
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


        [HttpPost("ImportExcel")]
        public IActionResult ImportPlayers(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                if (file == null || file.Length <= 0)
                {
                    return BadRequest("Invalid file.");
                }

                var fileExtension = Path.GetExtension(file.FileName)?.ToLower();
                var importedPlayers = new List<PlayerDTO>();
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    
                    using (var package = new ExcelPackage(file.OpenReadStream()))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var playerName = worksheet.Cells[row, 1].Text?.Trim();
                            var countryName = worksheet.Cells[row, 2].Text?.Trim();
                            var phoneNumber = worksheet.Cells[row, 3].Text?.Trim();
                            var email = worksheet.Cells[row, 4].Text?.Trim();
                            var levelText = worksheet.Cells[row, 5].Text?.Trim();
                             
                            var feeText = worksheet.Cells[row, 6].Text?.Trim();

                            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(countryName) ||
                                string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(levelText) ||
                                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(feeText))
                            {
                                continue;
                            }
                            bool fee;
                            if (feeText == "Rồi")
                            {
                                fee = true;

                            }
                            else
                            {
                                fee = false;
                            } 
                             
                             
                            if (!int.TryParse(levelText, out int level))
                            {
                                continue;
                            }

                            var player = new PlayerDTO
                            {
                                PlayerId = row,
                                PlayerName = playerName,
                                CountryName = countryName,
                                PhoneNumber = phoneNumber,
                                Email = email,
                                Level = level, 
                                IsPayed = fee
                            };

                            importedPlayers.Add(player);
                        }
                    }
                    return Ok(importedPlayers);
                }
                else
                {
                    return BadRequest("Unsupported file format. Please upload a valid Excel (.xls, .xlsx) file.");
                }

                
            }
            catch (IOException ex)
            {
                return BadRequest($"Error reading the file: {ex.Message}");
            }
            catch (Exception ex)
            {
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
