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

        // GET: api/Club
        [HttpGet]
        public ActionResult<IEnumerable<ClubDTO>> Get()
        {
            var clubs = _clubDAO.GetAllClubs();
            var clubsDto = _mapper.Map<List<ClubDTO>>(clubs);
            return Ok(clubsDto);
        }

        // GET: api/Club/5
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

        // POST: api/Club
        [HttpPost]
        public IActionResult Post([FromBody] ClubDTO clubDto)
        {
            if (clubDto == null)
            {
                return BadRequest();
            }

            var club = _mapper.Map<Club>(clubDto);

            // Ensure ClubId is not set explicitly, allowing the database to generate it
            club.ClubId = 0;

            _clubDAO.AddClub(club);

            return CreatedAtAction(nameof(Get), new { id = club.ClubId }, clubDto);
        }
        // PUT: api/Club/5
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

            // Use the ClubDTO properties to update the existingClub
            existingClub.ClubName = updatedClubDto.ClubName;
            existingClub.Address = updatedClubDto.Address;
            existingClub.Phone = updatedClubDto.Phone;
            existingClub.Facebook = updatedClubDto.Facebook;
            existingClub.Avatar = updatedClubDto.Avatar;

            // Update the existing entity
            _clubDAO.UpdateClub(existingClub);

            return NoContent();
        }




        // DELETE: api/Club/5
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
