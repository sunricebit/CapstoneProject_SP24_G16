using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PoolComVnWebAPI.Authentication;
using PoolComVnWebAPI.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using System.Text;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AccountDAO _accountDAO;

        public AuthenticationController(AccountDAO accountDAO)
        {
            _accountDAO = accountDAO;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginDTO loginDTO)
        {
            Account account = _accountDAO.AuthenAccount(loginDTO.Email, loginDTO.Password);
            if (account != null)
            {
                return Ok(new { token = TokenManager.GenerateToken(loginDTO.Email, account.RoleID) });
            }
            return Unauthorized();
        }

        //[AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterDTO registerDto)
        {
            if (_accountDAO.IsEmailExist(registerDto.email) || _accountDAO.IsUsernameExist(registerDto.username)) {
                return BadRequest();
            }
            _accountDAO.RegisterAccount(registerDto.username, registerDto.email, 
                registerDto.pass, registerDto.isBusiness);
            //if (account != null)
            //{
            //    return Ok(new { token = TokenManager.GenerateToken(username, account.RoleID) });
            //}
            return Ok();
        }

        [Authorize]
        [HttpPost("testAuthorize")]
        public IActionResult Authorize()
        {
            return Ok("authen thanh cong");
        }
    }
}
