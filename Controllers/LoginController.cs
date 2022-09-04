using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetDevTestAPI.Services;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BetDevTestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private  IUserServices _user;
        private IConfiguration Configuration { get; }

        public LoginController(ILogger<LoginController> logger, IConfiguration configuration, IUserServices user)
        {
            _logger = logger;
            _user = user;
            Configuration = configuration;

        }

        [HttpPost("Login")]
        public IActionResult LoginUser(string UserName,string Password)
        {
            try
            {
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {

                    


                    LoggedUsers loggedinuser = _user.GetLoggedInUser(UserName, Password);
                    if(loggedinuser is null)
                        return BadRequest("User not found");

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, loggedinuser.UserName),
                        new Claim(ClaimTypes.Email, loggedinuser.Email)
                    };

                    var token = new JwtSecurityToken
                    (
                        issuer: Configuration["Jwt:Issuer"],
                        audience: Configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        notBefore: DateTime.UtcNow,
                        signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                            SecurityAlgorithms.HmacSha256)
                    );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                    loggedinuser.Jwttoken = tokenString;

                
                    return Ok(tokenString);
                }
                return BadRequest("User not found");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error ShowShoppingCart");
            }
        }
}

}    
