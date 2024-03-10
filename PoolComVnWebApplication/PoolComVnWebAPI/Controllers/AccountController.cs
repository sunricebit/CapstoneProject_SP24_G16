using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly AccountDAO _accountDAO;

    public AccountController(AccountDAO accountDAO)
    {
        _accountDAO = accountDAO;
    }
    [HttpGet]
    public ActionResult<IEnumerable<AccountDTO>> Get()
    {
        try
        {
            var allAccounts = _accountDAO.GetAllAccounts();

            // Use LINQ to project each Account to AccountDTO
            var allAccountDTOs = allAccounts.Select(account => new AccountDTO
            {
                AccountID = account.AccountId,
                Email = account.Email,
                Password = account.Password,
                RoleID = account.RoleId,
                PhoneNumber = account.PhoneNumber,
                verifyCode = account.VerifyCode,
                Status = account.Status
                // Map other properties as needed
            });

            return Ok(allAccountDTOs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("ByUsername/{username}")]
    public ActionResult<AccountDTO> GetByUsername(string username)
    {
        try
        {
            var account = _accountDAO.GetAccountByUsername(username);

            if (account == null)
            {
                return NotFound();
            }

            // Map Account to AccountDTO
            var accountDTO = new AccountDTO
            {
                AccountID = account.AccountId,
                Email = account.Email,
                Password = account.Password,
                RoleID = account.RoleId,
                PhoneNumber = account.PhoneNumber,
                verifyCode = account.VerifyCode,
                Status = account.Status
                // Map other properties as needed
            };

            return Ok(accountDTO);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Post([FromBody] AccountDTO accountDTO)
    {
        try
        {
            // Use the constructor to create an Account instance from the DTO
            var account = new Account
            {
                // Map properties from DTO to entity
                AccountId = accountDTO.AccountID,
                Email = accountDTO.Email,
                Password = accountDTO.Password,
                RoleId = accountDTO.RoleID,
                PhoneNumber = accountDTO.PhoneNumber,
                VerifyCode = accountDTO.verifyCode,
                Status = accountDTO.Status
            };


            _accountDAO.AddAccount(account);

            // Use the constructor to create an AccountDTO instance from the entity
            var createdAccountDTO = new AccountDTO
            {
                // Map properties from entity to DTO
                AccountID = account.AccountId,
                Email = account.Email,
                Password = account.Password,
                RoleID = account.RoleId,
                PhoneNumber = account.PhoneNumber,
                verifyCode = account.VerifyCode,
                Status = account.Status
            };

            return CreatedAtAction(nameof(Get), new { id = createdAccountDTO.AccountID }, createdAccountDTO);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("Update/{id}")]
    public ActionResult Put(int id, [FromBody] AccountDTO updatedAccountDTO)
    {
        try
        {
            if (id != updatedAccountDTO.AccountID)
            {
                return BadRequest("Invalid Account ID");
            }

            // Map AccountDTO to Account
            var updatedAccount = new Account
            {
                AccountId = updatedAccountDTO.AccountID,
                Email = updatedAccountDTO.Email,
                Password = updatedAccountDTO.Password,
                RoleId = updatedAccountDTO.RoleID,
                PhoneNumber = updatedAccountDTO.PhoneNumber,
                VerifyCode = updatedAccountDTO.verifyCode,
                Status = updatedAccountDTO.Status
                // Map other properties as needed
            };

            _accountDAO.UpdateAccount(updatedAccount);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("Ban/{id}")]
    public ActionResult ToggleBanAccount(int id)
    {
        try
        {
            _accountDAO.ToggleBanAccount(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
