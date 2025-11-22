using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBakamitai.Data;
using ProjectBakamitai.DTO;
using ProjectBakamitai.Models;

namespace ProjectBakamitai.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BakamitaiController : ControllerBase
    {
        private readonly ProjectbakamitaiContext _db;
        protected ResponseApi _response;
        private byte[] HashPasswordToBytes(string password)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public BakamitaiController(ProjectbakamitaiContext db)
        {
            _db = db;
            _response = new();
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                // 1. Kiểm tra email tồn tại
                if (_db.Players.Any(p => p.Email == registerDTO.Email))
                {
                    _response.IsSucess = false;
                    _response.Notification = "Email already exists";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                // 2. Hash password
                var hashedPassword = HashPasswordToBytes(registerDTO.Password);

                // 3. Tạo Player mới
                var player = new Player
                {
                    PlayerName = registerDTO.PlayerName,
                    Email = registerDTO.Email,
                    PasswordHash = hashedPassword,
                    CreateDate = DateTime.UtcNow,
                };

                // 4. Lưu DB
                _db.Players.Add(player);
                await _db.SaveChangesAsync();

                // 5. Response thành công
                _response.IsSucess = true;
                _response.Notification = "Register successful";
                _response.Data = new
                {
                    player.PlayerId,
                    player.PlayerName,
                    player.Email,
                    player.CreateDate
                };

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.Notification = "Error";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var player = await _db.Players
                    .FirstOrDefaultAsync(p => p.Email == loginDTO.Email);

                if (player == null)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Email doesn't exist";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                byte[] hashedInputPassword = HashPasswordToBytes(loginDTO.Password);

                bool passwordMatch = hashedInputPassword.SequenceEqual(player.PasswordHash);
                if (!passwordMatch)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Wrong password";
                    _response.Data = null;
                    return BadRequest(_response);
                }
                
                _response.IsSucess = true;
                _response.Notification = "Login success";
                _response.Data = new
                {
                    player.PlayerId,
                    player.PlayerName,
                    player.Email,
                    player.CreateDate,
                };
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.Notification = "Server error";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpGet("GetItems")]
        public async Task<IActionResult> GetAllItem()
        {
            try
            {
                var items = await _db.Items.ToListAsync();
                _response.IsSucess = true;
                _response.Notification = "Gather Data successful";
                _response.Data = items;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.Notification = "Error";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

    }
}
