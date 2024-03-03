using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PoolComVnWebAPI.Authentication;
using PoolComVnWebAPI.Authorization;
using PoolComVnWebAPI.Common;
using PoolComVnWebAPI.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AccountDAO _accountDAO;
        private IConfiguration _config;
        private readonly IEmailSender _emailSender;

        public AuthenticationController(AccountDAO accountDAO, IConfiguration configuration, IEmailSender emailSender)
        {
            _accountDAO = accountDAO;
            _config = configuration;
            _emailSender = emailSender;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginDTO loginDTO)
        {
            Account account = _accountDAO.AuthenAccount(loginDTO.Email, loginDTO.Password);
            if (account != null)
            {
                return Ok(new { token = GenerateToken(account) });
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterDTO registerDto)
        {
            if (_accountDAO.IsEmailExist(registerDto.Email) || _accountDAO.IsUsernameExist(registerDto.Username)) {
                return BadRequest();
            }
            _accountDAO.RegisterAccount(registerDto.Username, registerDto.Email, 
                registerDto.Pass, registerDto.isBusiness);
            return Ok();
        }

        [HttpPost("testAuthorize")]
        [Authorize]
        public IActionResult Authorize()
        {
            // Lấy giá trị token từ header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Giải mã token để lấy các claims
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Xử lý logic của bạn với các claims
            var RoleClaim = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "Role");

            if (Constant.UserRole.ToString().Equals(RoleClaim.Value))
            {
                return Ok("authen thanh cong");
            }

            // Xử lý khi không tìm thấy claim "roleId"
            return BadRequest("Không tìm thấy thông tin về roleId trong token.");
        }

        private string GenerateToken(Account account)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("Role", account.RoleId.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims,
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        

        [HttpPost("verify")]
        public async Task<IActionResult> SendVerifyCode()
        {
            await _emailSender.SendMailAsync("Vhnam2209@gmail.com", "Nam Vu");
            return Ok();
        }
    }
}
