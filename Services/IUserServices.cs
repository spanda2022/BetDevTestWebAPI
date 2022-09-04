using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BetDevTestAPI.Services
{
   public  interface IUserServices
    {
        public  LoggedUsers GetLoggedInUser(string username, string password);
    }
}
