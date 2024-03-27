using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoloMatchController : ControllerBase
    {
        private readonly SoloMatchDAO _soloMatchDAO;
        public SoloMatchController(SoloMatchDAO soloMatchDAO)
        {
            _soloMatchDAO = soloMatchDAO;
        }
        [HttpGet("{solomatchId}")]
        public ActionResult<SoloMatchDTO> GetSoloMatch(int solomatchId)
        {
            var soloMatch = _soloMatchDAO.GetSoloMatchById(solomatchId);

            if (soloMatch == null)
            {
                return NotFound();
            }

            var soloMatchDTO = MapToDTO(soloMatch);
            return soloMatchDTO;
        }

        [HttpGet("ByClub/{clubID}")]
        public ActionResult<List<SoloMatchDTO>> GetAllSoloMatchByClubID(int clubID)
        {
            var soloMatches = _soloMatchDAO.GetAllSoloMatchByClubID(clubID);

            if (soloMatches == null || soloMatches.Count == 0)
            {
                return NotFound("No solo matches found for the provided club ID.");
            }

            var soloMatchDTOs = soloMatches.Select(s => MapToDTO(s)).ToList();
            return soloMatchDTOs;
        }

        [HttpPost]
        public IActionResult AddSoloMatch(SoloMatchDTO soloMatchDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var soloMatch = MapToEntity(soloMatchDTO);

            _soloMatchDAO.AddSoloMatch(soloMatch);

            return Ok(_soloMatchDAO.GetLastestSoloMatch().SoloMatchId);
        }
        private SoloMatchDTO MapToDTO(SoloMatch soloMatch)
        {
            return new SoloMatchDTO
            {
                SoloMatchId = soloMatch.SoloMatchId,
                StartTime = soloMatch.StartTime,
                GameTypeId = soloMatch.GameTypeId,
                ClubId = soloMatch.ClubId,
                Description = soloMatch.Description,
                Status = soloMatch.Status,
                Flyer = soloMatch.Flyer,
                RaceTo = soloMatch.RaceTo
            };
        }
        private SoloMatch MapToEntity(SoloMatchDTO soloMatchDTO)
        {
            return new SoloMatch
            {
                StartTime = soloMatchDTO.StartTime,
                GameTypeId = soloMatchDTO.GameTypeId,
                ClubId = soloMatchDTO.ClubId,
                Description = soloMatchDTO.Description,
                Status = soloMatchDTO.Status,
                Flyer = soloMatchDTO.Flyer,
                RaceTo = soloMatchDTO.RaceTo
            };
        }
    }
}
    


