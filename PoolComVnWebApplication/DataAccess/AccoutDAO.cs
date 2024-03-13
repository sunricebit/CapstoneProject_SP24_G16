using BCrypt.Net;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DataAccess
{
    public class AccountDAO
    {
        private readonly poolcomvnContext _context;

        public AccountDAO(poolcomvnContext poolComContext)
        {
            _context = poolComContext;
        }

        /// <summary>
        /// Ban or unban an account by toggling its Status property.
        /// </summary>
        public void ToggleBanAccount(int accountId)
        {
            try
            {
                var accountToToggle = _context.Accounts.Find(accountId);

                if (accountToToggle != null)
                {
                    // Toggle Status (ban if unbanned, unban if banned)
                    accountToToggle.Status = !accountToToggle.Status;

                    _context.SaveChanges();
                }
                else
                {
                    // Account not found
                    throw new InvalidOperationException("Account not found in the database.");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to toggle ban status: {e.Message}", e);
            }
        }


        /// <summary>
        /// Register a new account.
        /// </summary>
        public void RegisterAccount(string username, string email, string pass, bool isBussiness)
        {
            try
            {
                Account account = new Account()
                {
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(pass, Constant.SaltRound),
                    RoleId = isBussiness ? Constant.BusinessRole : Constant.UserRole,
                    PhoneNumber = "Default",
                    Status = false,
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
        /// Get all accounts from the database.
        /// </summary>
        public List<Account> GetAllAccounts()
        {
            try
            {
                return _context.Accounts.ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve accounts: {e.Message}", e);
            }
        }

        /// <summary>
        /// Add a new account to the database.
        /// </summary>
        public void AddAccount(Account newAccount)
        {
            try
            {
                _context.Accounts.Add(newAccount);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to add account: {e.Message}", e);
            }
        }

        /// <summary>
        /// Update an existing account in the database.
        /// </summary>
        public void UpdateAccount(Account updatedAccount)
        {
            try
            {
                var existingAccount = _context.Accounts.Find(updatedAccount.AccountId);

                if (existingAccount != null)
                {
                    existingAccount.Email = updatedAccount.Email;
                    existingAccount.Password = updatedAccount.Password; // Ensure to hash the updated password if needed
                    existingAccount.RoleId = updatedAccount.RoleId;
                    existingAccount.PhoneNumber = updatedAccount.PhoneNumber;
                    existingAccount.VerifyCode = updatedAccount.VerifyCode;
                    existingAccount.Status = updatedAccount.Status;

                    _context.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException("Account not found in the database.");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to update account: {e.Message}", e);
            }
        }

        /// <summary>
        /// Authentication Account
        /// </summary>
        public Account? AuthenAccount(string username, string pass)
        {
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

        /// <summary>
        /// Get an account by username from the database and determine the account type.
        /// </summary>
        public Account GetAccountByUsername(string username)
        {
            try
            {
                var account = _context.Accounts
                    .Include(a => a.Club) // Include the Club navigation property
                    .FirstOrDefault(item => username.Equals(item.Email));

                if (account != null)
                {
                    // Check if the account is associated with a club (business account)
                    if (account.Club != null)
                    {
                        // Business account
                        return account;
                    }
                    else
                    {
                        // User account
                        return account;
                    }
                }
                else
                {
                    // Account not found
                    throw new InvalidOperationException("Account not found in the database.");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve account: {e.Message}", e);
            }
        }

        /// <summary>
        /// Get an account 
        /// </summary>
        public Account GetAccountByEmail(string email)
        {
            try
            {
                var account = _context.Accounts
                    .FirstOrDefault(item => email.Equals(item.Email));

                return account;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve account: {e.Message}", e);
            }
        }

        /// <summary>
        /// Get an account 
        /// </summary>
        public Account GetAccountById(int accountId)
        {
            try
            {
                var account = _context.Accounts.FirstOrDefault(a => a.AccountId.Equals(accountId));

                return account;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve account: {e.Message}", e);
            }
        }

        /// <summary>
        /// Check if a username exists.
        /// </summary>
        public bool IsUsernameExist(string username)
        {
            var account = _context.Accounts.FirstOrDefault(item => username.Equals(item.Email));
            return account != null;
        }

        /// <summary>
        /// Check if an email exists.
        /// </summary>
        public bool IsEmailExist(string email)
        {
            var account = _context.Accounts.FirstOrDefault(item => email.Equals(item.Email));
            return account != null;
        }

        public int CheckAccountStatus(int accountId)
        {
            var account = _context.Accounts.FirstOrDefault(item => item.AccountId.Equals(accountId));
            if (account.Status)
            {
                return Constant.AccountStatusReady;
            }
            else return string.IsNullOrEmpty(account.VerifyCode) ? Constant.AccountStatusBanned : Constant.AccountStatusVerify;
        }

        public bool CheckVerifyAccount(int accountId, string verifyCode)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId.Equals(accountId));
            if (verifyCode == account.VerifyCode)
            {
                account.VerifyCode = null;
                return true;
            }
            return false;
        }

        public Account GetLastestAccount()
        {
            try
            {
                var account = _context.Accounts.OrderByDescending(a => a.AccountId).FirstOrDefault();

                return account;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SetVerifyCode(int accountId, string verifyCode)
        {
            try
            {
                var account = _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                account.VerifyCode = verifyCode;
                _context.Accounts.Update(account);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
