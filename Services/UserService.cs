using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetDevTestAPI.Services;
using Microsoft.EntityFrameworkCore;
using BetDevTestAPI.Data;
using System.Security.Cryptography;

namespace BetDevTestAPI.Services
{
    public class UserService : IUserServices
    {
        private ApplicationDBContext _db;
        public UserService(ApplicationDBContext db)
        {
            _db = db;
        }
        public LoggedUsers GetLoggedInUser(string username, string password)
        {

            Users users = _db.Users.FirstOrDefault(u => u.UserName == username);
            LoggedUsers lUser = new LoggedUsers();
            lUser.UserName = users.UserName;
            lUser.Email = users.Email;
            lUser.Jwttoken = "";
            return lUser;

        }

      
    }
}
