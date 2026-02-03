using BCrypt.Net;
using LoggAutorz.DataBase;
using LoggAutorz.Migrations;
using LoggAutorz.Repositorie;
using LoggAutorz.ServicesDb;
using LoggAutorz.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Data;
using System.Net.Mail;
using System.Text.Json.Serialization;

namespace LoggAutorz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private ServicesToApiHttp _services;
        private readonly GenerateService _generateService;

        public UserController(AppDbContext appDbContext, ServicesToApiHttp services, GenerateService generateService)
        {
            _appDbContext = appDbContext;
            _services = services;
            _generateService = generateService;
        }

        //static OpenApiMediaType type = new OpenApiMediaType();
        //public static readonly IOpenApiMediaType iOpenApiMediaType = type;

        //------------------------------
        //[Authorize("AdminMax")]
        //[Authorize("Admin")]

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

                try
            {
                if (string.IsNullOrEmpty(dto.Password) || dto.Password.Length < 8)
                    return BadRequest("Password must be at least 8 characters long");
                if (string.IsNullOrEmpty(dto.Name))
                {
                    return BadRequest("Invalid username, cannot be null");
                }
                if (string.IsNullOrWhiteSpace(dto.Email) || !MailAddress.TryCreate(dto.Email, out _))
                {
                    return BadRequest("Invalid email format");
                }
                if (await _appDbContext.UserAccounts.AnyAsync(u => u.Email == dto.Email))
                {
                    return BadRequest("Email already in use");
                }

                var user = new UsersEntity
                {
                    UserName = dto.Name,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    totalTry = 3,
                    role = dto.role
                };
                //if (user.role == new[] { "Employee" }) {}


                _appDbContext.UserAccounts.Add(user);
                await _appDbContext.SaveChangesAsync();
                var response = new
                {
                    Id = user.UserId,
                    Name = user.UserName,
                    Email = user.Email,
                    role = user.role
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
                //return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }



        //------------------------------
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dtoLogin)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //throw new NullReferenceException("Can't be null");
            var user = await _appDbContext.UserAccounts.FirstOrDefaultAsync(u => u.Email == dtoLogin.Email);
            bool valid = BCrypt.Net.BCrypt.Verify(dtoLogin.Password, user.PasswordHash);

            try
            {
                if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(dtoLogin.Password) || !valid)
                {

                    user.totalTry--;
                    await _appDbContext.SaveChangesAsync();
                    if (user.totalTry == 0)
                    {
                        return BadRequest("Try again later");
                    }
                    return Conflict("Try again " + $"{user.totalTry}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);

            }
            user.totalTry = 3;
            await _appDbContext.SaveChangesAsync();

            return Ok($"Hello, {user.UserName}! How are you?");

        }

            }
    }
    