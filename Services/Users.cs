using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace BetDevTestAPI.Services
{
    public class Users : IdentityUser
    {
        
        public override string UserName { get; set; }

        public override string Email { get; set; }
        public override string PasswordHash { get; set; }
        
     

    }

    public class LoggedUsers 
    {

        public  string UserName { get; set; }

        public  string Email { get; set; }
        

          public string Jwttoken { get; set; }


    }
}
