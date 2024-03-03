using System;
using System.Collections.Generic;
using AutoMapper;
using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubPostController : ControllerBase
    {
        private readonly ClubPostDAO _clubPostDAO;
        private readonly IMapper _mapper;

        public ClubPostController(ClubPostDAO clubPostDAO, IMapper mapper)
        {
            _clubPostDAO = clubPostDAO;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ClubPostDTO>> Get()
        {
            var clubPosts = _clubPostDAO.GetAllClubPosts();
            var clubPostsDto = _mapper.Map<List<ClubPostDTO>>(clubPosts);
            return Ok(clubPostsDto);
        }

        [HttpGet("{id}")]
        public ActionResult<ClubPostDTO> Get(int id)
        {
            var clubPost = _clubPostDAO.GetClubPostById(id);

            if (clubPost == null)
            {
                return NotFound();
            }

            var clubPostDto = _mapper.Map<ClubPostDTO>(clubPost);
            return Ok(clubPostDto);
        }

        [Authorize] // Bắt buộc người dùng đăng nhập
        [HttpPost]
        public IActionResult Post([FromBody] ClubPostDTO clubPostDto)
        {
            if (clubPostDto == null)
            {
                return BadRequest();
            }

            // Lấy ClubID từ JWT Token
            var clubIdFromToken = GetClubIdFromToken(); // Hàm giả định, bạn cần triển khai hàm này

            // Kiểm tra xem ClubID từ Token có khớp với ClubID từ DTO không
            if (clubPostDto.ClubID != clubIdFromToken)
            {
                return BadRequest("Invalid ClubID in the request.");
            }

            var clubPost = _mapper.Map<ClubPost>(clubPostDto);
            clubPost.PostId = 0; // Ensure PostID is not set explicitly

            _clubPostDAO.AddClubPost(clubPost);

            var createdClubPostDto = _mapper.Map<ClubPostDTO>(clubPost);
            return CreatedAtAction(nameof(Get), new { id = createdClubPostDto.PostID }, createdClubPostDto);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ClubPostDTO updatedClubPostDto)
        {
            if (updatedClubPostDto == null || id != updatedClubPostDto.PostID)
            {
                return BadRequest();
            }

            var existingClubPost = _clubPostDAO.GetClubPostById(id);

            if (existingClubPost == null)
            {
                return NotFound();
            }

            // Lấy ClubID từ JWT Token
            var clubIdFromToken = GetClubIdFromToken(); // Hàm giả định, bạn cần triển khai hàm này

            // Kiểm tra xem ClubID từ Token có khớp với ClubID từ DTO không
            if (updatedClubPostDto.ClubID != clubIdFromToken)
            {
                return BadRequest("Invalid ClubID in the request.");
            }

            _mapper.Map(updatedClubPostDto, existingClubPost);
            _clubPostDAO.UpdateClubPost(existingClubPost);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var clubPost = _clubPostDAO.GetClubPostById(id);

            if (clubPost == null)
            {
                return NotFound();
            }

            // Lấy ClubID từ JWT Token
            var clubIdFromToken = GetClubIdFromToken(); // Hàm giả định, bạn cần triển khai hàm này

            // Kiểm tra xem ClubID từ Token có khớp với ClubID của bản ghi không
            if (clubPost.ClubId != clubIdFromToken)
            {
                return BadRequest("Invalid ClubID in the request.");
            }

            _clubPostDAO.DeleteClubPost(id);

            return NoContent();
        }

        // Hàm giả định để lấy ClubID từ JWT Token
        private int GetClubIdFromToken()
        {
            var userIdClaim = HttpContext.User.FindFirst("ClubId");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var clubId))
            {
                return clubId;
            }

            throw new InvalidOperationException("ClubId claim not found or invalid in the JWT token.");
        }
    }
}
