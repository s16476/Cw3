using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.DAL;
using api.DTOs;
using api.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers
{
    [ApiController]
    [Route("api/login")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {

        private readonly IDbService _dbService;

        private IConfiguration Configuration { get; set; }

        public LoginController(IDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            Configuration = configuration;
        }


        [HttpPost]
        public IActionResult Login(LoginRequestDto requestDto)
        {

            var claims = new Claim[0];

            if (requestDto.Login == "admin" && requestDto.Password == "ASDF1234")
            {
                claims = new[]
               {
                new Claim(ClaimTypes.NameIdentifier, "admin"),
                new Claim(ClaimTypes.Name, "Administrator"),
                new Claim(ClaimTypes.Role, "employee"),
                new Claim(ClaimTypes.Role, "admin")
                };
            }
            else if (requestDto.Login == "emp" && requestDto.Password == "ASDF")
            {
                claims = new[]
               {
                new Claim(ClaimTypes.NameIdentifier, "emp"),
                new Claim(ClaimTypes.Name, "Employee"),
                new Claim(ClaimTypes.Role, "employee")
                };
            }
            else
            {
                var student = _dbService.FindStudentToLogin(requestDto.Login, requestDto.Password);

                claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, student.IndexNumber),
                new Claim(ClaimTypes.Name, student.FirstName + " " + student.LastName),
                new Claim(ClaimTypes.Role, "student")
                };
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "Gakko",
                audience: "students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
                );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });
        }




    }
}