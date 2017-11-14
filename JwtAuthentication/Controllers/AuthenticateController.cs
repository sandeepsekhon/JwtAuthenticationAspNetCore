using JwtAuthentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JwtAuthentication.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticateController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticateController> _logger;

        public AuthenticateController(IConfiguration configuration, ILogger<AuthenticateController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        
        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]TokenRequest request)
        {
            _logger.LogInformation($"Request for token came for user {request?.UserName} at {DateTime.Now}");

            if(request.UserName=="admin@test.com" && request.Password=="Admin123!")
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.UserName)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "mydomain.com",
                    audience: "mydomain.com",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new
                {
                    Name = "Admin",
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

            return BadRequest("Could not verify username and password");
        }

    }
}
