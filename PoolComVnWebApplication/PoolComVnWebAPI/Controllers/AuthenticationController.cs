using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PoolComVnWebAPI.Authentication;
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
        public IActionResult Login(string username, string pass)
        {
            Account account = _accountDAO.AuthenAccount(username, pass);
            if (account != null)
            {
                return Ok(new { token = TokenManager.GenerateToken(username, account.RoleID) });
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]string username, string email, string pass, bool isBusiness)
        {
            if (_accountDAO.IsEmailExist(email) || _accountDAO.IsUsernameExist(username)) {
                return BadRequest();
            }
            _accountDAO.RegisterAccount(username, email, pass, isBusiness);
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
            return Ok();
        }
    }
}
