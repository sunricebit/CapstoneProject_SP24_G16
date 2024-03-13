using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        public AuthorizationController()
        {

        }

        [HttpPost]
        public IActionResult CheckAuthorization(List<int> rolesAccess)
        {
            // Lấy giá trị token từ header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Giải mã token để lấy các claims
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Xử lý logic với các claims
            var RoleClaim = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "Role");

            // Xử lý khi không tìm thấy claim "roleId"
            if (RoleClaim == null)
            {
                return Unauthorized();
            }

            foreach (var role in rolesAccess)
            {
                if (role.ToString().Equals(RoleClaim.Value))
                {
                    return Ok();
                }
            }
            
            // Không có quyền access
            return Forbid();
        }
    }
}
