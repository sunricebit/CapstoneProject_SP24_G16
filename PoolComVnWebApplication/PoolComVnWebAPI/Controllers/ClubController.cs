using AutoMapper;
using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;
using System.Collections.Generic;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly ClubDAO _clubDAO;
        private readonly IMapper _mapper;

        public ClubController(ClubDAO clubDAO, IMapper mapper)
        {
            _clubDAO = clubDAO;
            _mapper = mapper;
        }

        
        [HttpGet]
        public ActionResult<IEnumerable<ClubDTO>> Get()
        {
            var clubs = _clubDAO.GetAllClubs();
            var clubsDto = _mapper.Map<List<ClubDTO>>(clubs);
            return Ok(clubsDto);
        }
        [HttpGet("Matches")]
        public ActionResult<IEnumerable<MatchesDTO>> Get2()
        {
            var matches = _clubDAO.matchOfTournaments();
            List<MatchesDTO> matchesDTOs = new List<MatchesDTO>();
            foreach( var match in matches)
            {
                MatchesDTO matche2 = new MatchesDTO
                {
                    MatchId = match.MatchId,
                    PlayerInMatches = match.PlayerInMatches,
                };
                matchesDTOs.Add(matche2);
            }
                

            return Ok(matchesDTOs);
        }


        
        [HttpGet("{id}")]
        public ActionResult<ClubDTO> Get(int id)
        {
            var club = _clubDAO.GetClubById(id);

            if (club == null)
            {
                return NotFound();
            }

            var clubDto = _mapper.Map<ClubDTO>(club);
            return Ok(clubDto);
        }


        [HttpPost("Add")]
        public ActionResult Post([FromBody] ClubDTO clubDTO)
        {
            try
            {
               
                var existingClub = _clubDAO.GetClubByName(clubDTO.ClubName);

                if (existingClub != null)
                {
                    return BadRequest("Tên câu lạc bộ đã tồn tại trong hệ thống.");
                }
                int accountId = clubDTO.AccountId ?? 0;
                var account = _clubDAO.GetAccount(accountId);

                var club = new Club
                {
                    ClubName = clubDTO.ClubName,
                    Facebook = clubDTO.Facebook,
                    Phone = clubDTO.Phone,
                    Address = clubDTO.Address,
                    Avatar = clubDTO.Avatar,
                    AccountId = clubDTO.AccountId,
                    Status = clubDTO.Status,
                    Account = account
                };
                _clubDAO.AddClub(club);
                return CreatedAtAction(nameof(Get), new { id = club.ClubId }, clubDTO);
            }
            catch (Exception ex)
            {
               
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetClubByAccountId")]
        public ActionResult<ClubDTO> GetClubByAccountId(int accountID)
        {
            try
            {
                var club = _clubDAO.GetClubByAccountId(accountID);

                if (club == null)
                {
                    return NotFound("Không tìm thấy câu lạc bộ cho AccountId đã cung cấp.");
                }

                var clubDto = new ClubDTO
                {
                    ClubId = club.ClubId,
                    ClubName = club.ClubName,
                    Facebook = club.Facebook,
                    Phone = club.Phone,
                    Address = club.Address,
                    Avatar = club.Avatar,
                    AccountId = club.AccountId,
                    Status = club.Status
                };

                return Ok(clubDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy câu lạc bộ: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ClubDTO updatedClubDto)
        {
            if (updatedClubDto == null)
            {
                return BadRequest("Invalid request data");
            }

            var existingClub = _clubDAO.GetClubById(id);

            if (existingClub == null)
            {
                return NotFound();
            }
            existingClub.ClubName = updatedClubDto.ClubName;
            existingClub.Address = updatedClubDto.Address;
            existingClub.Phone = updatedClubDto.Phone;
            existingClub.Facebook = updatedClubDto.Facebook;
            existingClub.Avatar = updatedClubDto.Avatar;
            _clubDAO.UpdateClub(existingClub);

            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var club = _clubDAO.GetClubById(id);

            if (club == null)
            {
                return NotFound();
            }

            _clubDAO.DeleteClub(id);

            return NoContent();
        }
    }
}
