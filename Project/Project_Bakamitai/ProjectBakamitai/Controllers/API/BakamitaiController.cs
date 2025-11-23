using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ProjectBakamitai.Data;
using ProjectBakamitai.DTO;
using ProjectBakamitai.Models;
using System.Security.Cryptography;
using System.Text;

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
                if (_db.Players.Any(p => p.Email == registerDTO.Email))
                {
                    _response.IsSucess = false;
                    _response.Notification = "Email already exists";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                var hashedPassword = HashPasswordToBytes(registerDTO.Password);

                var player = new Player
                {
                    PlayerName = registerDTO.PlayerName,
                    Email = registerDTO.Email,
                    PasswordHash = hashedPassword,
                    CreateDate = DateTime.UtcNow,
                };

                _db.Players.Add(player);
                await _db.SaveChangesAsync();

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

        [HttpGet("GetItemsByType/{type}")]
        public async Task<IActionResult> GetItemsByType(string type)
        {
            try
            {
                var items = await _db.Items
                    .Where(i => i.ItemType == type)
                    .ToListAsync();

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

        [HttpPost("CreateCharacter")]
        public async Task<IActionResult> CreateCharacter(CreateCharactersDTO createCharactersDTO)
        {
            try
            {
                var player = await _db.Players.FindAsync(createCharactersDTO.PlayerId);
                if (player == null)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Player not found";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                var mode = await _db.GameModes.FindAsync(createCharactersDTO.GamemodeId);
                if (mode == null)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Gamemode not found";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                var existingCharacter = await _db.Characters
                    .FirstOrDefaultAsync(c => c.PlayerId == createCharactersDTO.PlayerId && c.CharacterName == createCharactersDTO.CharacterName);
                if (existingCharacter != null)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Character name already exists for this player";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                var character = new Character
                {
                    PlayerId = createCharactersDTO.PlayerId,
                    CharacterName = createCharactersDTO.CharacterName,
                    GamemodeId = createCharactersDTO.GamemodeId,
                };
                _db.Characters.Add(character);
                await _db.SaveChangesAsync();
                _response.IsSucess = true;
                _response.Notification = "Character created successfully";
                _response.Data = new
                {
                    CharacterID = character.CharacterId,
                    CharacterName = character.CharacterName,
                    PlayerID = character.PlayerId,
                    GamemodeID = character.GamemodeId
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

        [HttpGet("GetCharactersByPlayer/{PlayerName}")]
        public async Task<IActionResult> GetCharactersByPlayer(string PlayerName)
        {
            try
            {
                var playername = await _db.Players
                    .FirstOrDefaultAsync(p => p.PlayerName.ToLower() == PlayerName.ToLower());
                if(playername == null)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Player not found";
                    return NotFound(_response);
                }
                var characters = await _db.Characters
                    .Where(c => c.PlayerId == playername.PlayerId)
                    .Include(c => c.Player)
                    .Include(c => c.Gamemode)
                    .Select(c => new CharacterResponseDTO
                    {
                        CharacterId = c.CharacterId,
                        CharacterName = c.CharacterName,
                        PlayerName = c.Player.PlayerName,
                        GamemodeName = c.Gamemode.ModeName
                    })
                    .ToListAsync();

                _response.IsSucess = true;
                _response.Notification = "Gather Data successful";
                _response.Data = characters;

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

        [HttpGet("GetCharacterByGamemode/{ModeName}")]
        public async Task<IActionResult> GetCharacterByGamemode(string ModeName)
        {
            try
            {
                var gamemode = await _db.GameModes
                    .FirstOrDefaultAsync(g => g.ModeName.ToLower() == ModeName.ToLower());

                if (gamemode == null)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Gamemode not found";
                    return NotFound(_response);
                }
                var characters = await _db.Characters
                    .Where(c => c.GamemodeId == gamemode.GamemodeId)
                    .Include(c => c.Player)
                    .Include(c => c.Gamemode)
                    .Select(c => new CharacterResponseDTO
                    {
                        CharacterId = c.CharacterId,
                        CharacterName = c.CharacterName,
                        PlayerName = c.Player.PlayerName,
                        GamemodeName = c.Gamemode.ModeName
                    })
                    .ToListAsync();
                _response.IsSucess = true;
                _response.Notification = "Gather Data successful";
                _response.Data = characters;
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
