using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class UserDAO
    {
        private readonly poolcomvnContext _context;

        public UserDAO(poolcomvnContext poolComContext)
        {
            _context = poolComContext;
        }

        public List<User> GetAllUsers()
        {
            try
            {
               var users = _context.Users.Include(u => u.Account).ToList();
               return users;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve accounts: {e.Message}", e);
            }
        }
    }
}
