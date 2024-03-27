using DataAccess;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDAO _userDAO;
        private readonly AccountDAO _accountDAO;
        private readonly AddressDAO _addressDAO;
        public UserController(UserDAO userDAO,AccountDAO accountDAO, AddressDAO addressDAO)
        {
            _userDAO = userDAO;
            _accountDAO = accountDAO;
            _addressDAO = addressDAO;
        }

        [HttpGet("GetAllUser")]
        public ActionResult<IEnumerable<UserDTO>> GetAllUsers()
        {
            try
            {
                var users = _userDAO.GetAllUsers();

                var userDTOs = users.Select(user => new UserDTO
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Address = user.Address,
                    Avatar = user.Avatar,
                    Dob = user.Dob,
                    CreatedDate = user.CreatedDate,
                    UpdatedDate = user.UpdatedDate,
                    AccountId = user.AccountId,
                    WardCode = user.WardCode
                });

                return Ok(userDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
