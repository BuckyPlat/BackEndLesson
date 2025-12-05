using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projectbakamitai.Data;
using Projectbakamitai.DTO;
using Projectbakamitai.Models;

namespace Projectbakamitai.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameAPIController : ControllerBase
    {
        private readonly ProjectBakamitaiContext _context;
        protected ResponseAPI _response;
        private byte[] HashPasswordToBytes(string password)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        public GameAPIController(ProjectBakamitaiContext context)
        {
            _context = context;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                if (_context.Users.Any(u => u.Email == registerDTO.Email))
                {
                    return BadRequest(new ResponseAPI
                    {
                        IsSuccess = false,
                        Notification = "Email already exists"
                    });
                }

                // Hash password
                var hashed = HashPasswordToBytes(registerDTO.Password);

                // Create User
                var user = new User
                {
                    UserName = registerDTO.UserName,
                    Email = registerDTO.Email,
                    PasswordHash = hashed
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();   // necessary to generate UserId

                // Prevent duplicate PlayerProfile creation
                if (!_context.PlayerProfiles.Any(p => p.UserId == user.UserId))
                {
                    var profile = new PlayerProfile
                    {
                        UserId = user.UserId,
                        DisplayName = user.UserName,
                        AvatarUrl = "/images/avatars/default.png"
                    };

                    _context.PlayerProfiles.Add(profile);
                    await _context.SaveChangesAsync();
                }

                return Ok(new ResponseAPI
                {
                    IsSuccess = true,
                    Notification = "Register Successful",
                    Data = new
                    {
                        user.UserId,
                        user.UserName,
                        user.Email,
                        user.CreateAt
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI
                {
                    IsSuccess = false,
                    Notification = "Error",
                    Data = ex.InnerException?.Message ?? ex.Message
                });
            }
        }


    }
}
