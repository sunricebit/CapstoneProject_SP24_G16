using BCrypt.Net;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AccountDAO
    {
        private readonly PoolComContext _context;

        public AccountDAO(PoolComContext poolComContext)
        {
            _context = poolComContext;
        }

        /// <summary>
        /// 
        /// </summary>
        public void BanAccount() { }

        /// <summary>
        /// Register 
        /// </summary>
        public void RegisterAccount(string username, string email, string pass, bool isBussiness) {
            try
            {
                Account account = new Account()
                {
                    Email = email,
                  
                    Password = BCrypt.Net.BCrypt.HashPassword(pass, Constant.SaltRound),
                    RoleID = isBussiness ? Constant.BusinessRole : Constant.UserRole,
                };
                _context.Accounts.Add(account);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetAllAccount() { }

        /// <summary>
        /// Authentication Account
        /// </summary>
        public Account? AuthenAccount(string username, string pass) {
            var account = _context.Accounts.FirstOrDefault(account => account.Email.Equals(username));
            if (account != null)
            {
                bool verify = BCrypt.Net.BCrypt.Verify(pass, account.Password);
                if (verify)
                {
                    return account;
                }
                else return null;
            }

            return account;
        }

        public Account? GetAccountByUsername(string username)
        {
            Account? account = _context.Accounts.FirstOrDefault(item => username.Equals(item.Email));
            return account;
        }

        public bool IsUsernameExist(string username)
        {
            Account? account = _context.Accounts.FirstOrDefault(item => username.Equals(item.Email));
            return account == null ? false : true;
        }

        public bool IsEmailExist(string email)
        {
            Account? account = _context.Accounts.FirstOrDefault(item => email.Equals(item.Email));
            return account == null ? false : false;
        }
    }
}