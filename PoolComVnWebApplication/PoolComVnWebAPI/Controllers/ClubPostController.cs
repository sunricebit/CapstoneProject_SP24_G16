using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using AutoMapper;
using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var clubPost2 = new ClubPost
            {
                PostId = clubPost.PostId,
                ClubId = clubPost.ClubId,
                Title = clubPost.Title,
                Description = clubPost.Description,
                CreatedDate = clubPost.CreatedDate,
                UpdatedDate = clubPost.UpdatedDate,
                Flyer = clubPost.Flyer,
                Link = clubPost.Link,
                Status = clubPost.Status,
            }; ;
            return Ok(clubPost2);
        }
        [HttpGet("ChangeStatus")]
        public ActionResult<ClubPostDTO> ChangeStatus(int id)
        {
            var clubPost = _clubPostDAO.GetClubPostById(id);

            if (clubPost == null)
            {
                return NotFound();
            }

            if (clubPost.Status == true) { clubPost.Status = false; }
            else
            {
                clubPost.Status = true;
            }
            _clubPostDAO.UpdateClubPost(clubPost);    
            return Ok();
        }

        [HttpPost("Add")]
        public IActionResult Post([FromBody] ClubPostDTO clubPostDTO)
        {
            try
            {
              
                var clubPost = new ClubPost
                {
                    ClubId = clubPostDTO.ClubID,
                    Title = clubPostDTO.Title,
                    Description = clubPostDTO.Description,
                    CreatedDate = clubPostDTO.CreatedDate,
                    UpdatedDate = clubPostDTO.UpdatedDate,
                    Flyer = clubPostDTO.Flyer,
                    Link = clubPostDTO.Link,
                    Status = true
                };


                _clubPostDAO.AddClubPost(clubPost);
                return Ok(clubPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetByClubId/{clubId}")]
        public IActionResult GetByClubId(int clubId)
        {
            try
            {
                var clubPosts = _clubPostDAO.GetClubPostByClubId(clubId);

                if (clubPosts == null)
                {
                    return NotFound("Câu lạc bộ chưa có bài post nào");
                }
                return Ok(clubPosts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


      
    }
}
