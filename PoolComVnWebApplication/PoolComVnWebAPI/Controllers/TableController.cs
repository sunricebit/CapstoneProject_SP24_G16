﻿using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BusinessObject.Models;
using DataAccess;
using PoolComVnWebAPI.DTO;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly TableDAO _tableDAO;
        private readonly ClubDAO _clubDAO;
        private readonly IMapper _mapper;

        public TableController(TableDAO tableDAO, IMapper mapper, ClubDAO clubDAO)
        {
            _tableDAO = tableDAO;
            _clubDAO = clubDAO;
            _mapper = mapper;
        }

        [HttpGet("GetAllTablesForClub")]
        [Authorize]
        public IActionResult GetAllTablesForClub()
        {
            //Lấy giá trị token từ header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Giải mã token để lấy các claims
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Xử lý logic của bạn với các claims
            var roleClaim = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Role"));
            var account = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Account"));
            if (!Constant.BusinessRole.ToString().Equals(roleClaim?.Value))
            {
                return BadRequest("Unauthorized");
            }

            if (!Int32.TryParse(account?.Value, out int accountId))
            {
                return BadRequest("Invalid AccountId claim");
            }

            int clubId = _clubDAO.GetClubIdByAccountId(accountId);

            var tables = _tableDAO.GetAllTablesForClub(clubId);

            // Map to TableDTO and return
            var tableDTOs = _mapper.Map<List<TableDTO>>(tables);
            return Ok(tableDTOs);
        }


        // GET: api/Table/5
        [HttpGet("GetTableBy/{id}")]
        public IActionResult GetTableById(int id)
        {
            var table = _tableDAO.GetTableById(id);

            if (table == null)
            {
                return NotFound();
            }

            var tableDTO = _mapper.Map<TableDTO>(table);
            return Ok(tableDTO);
        }

        // POST: api/Table
        [HttpPost("AddTable")]
        public IActionResult AddTable([FromBody] TableDTO tableDTO)
        {

            //Lấy giá trị token từ header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid token");
            }
            // Giải mã token để lấy các claims
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Xử lý logic của bạn với các claims
            var roleClaim = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Role"));
            var account = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Account"));
            if (!Constant.BusinessRole.ToString().Equals(roleClaim?.Value))
            {
                return BadRequest("Unauthorized");
            }

            if (!Int32.TryParse(account?.Value, out int accountId))
            {
                return BadRequest("Invalid AccountId claim");
            }

            int clubId = _clubDAO.GetClubIdByAccountId(accountId);

            if (tableDTO == null)
            {
                return BadRequest("TableDTO cannot be null");
            }

            var table = _mapper.Map<Table>(tableDTO);
            _tableDAO.AddTable(table, clubId);

            return CreatedAtAction("GetTableById", new { id = table.TableId }, tableDTO);
        }

        // PUT: api/Table/5
        [HttpGet("Update/{id}")]
        public IActionResult UpdateTable(int id, [FromBody] TableDTO updatedTableDTO)
        {
            var existingTable = _tableDAO.GetTableById(id);

            if (existingTable == null)
            {
                return NotFound();
            }

            _mapper.Map(updatedTableDTO, existingTable);
            _tableDAO.UpdateTable(existingTable);

            return NoContent();
        }

        // DELETE: api/Table/5
        [HttpGet("Delete/{id}")]
        public IActionResult DeleteTable(int id)
        {
            var existingTable = _tableDAO.GetTableById(id);

            if (existingTable == null)
            {
                return NotFound();
            }

            _tableDAO.DeleteTable(id);

            return NoContent();
        }
    }
}
