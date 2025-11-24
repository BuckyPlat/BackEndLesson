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
                if (playername == null)
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
        // YÊU CẦU 3: Lấy items có tên chứa "kim cương" và ExpValue < 500
        [HttpGet("DiamondItems")]
        public async Task<IActionResult> GetDiamondItems()
        {
            try
            {
                var items = await _db.Items
                    .Where(i => i.ItemName.Contains("kim cương") && i.ExpValue < 500)
                    .OrderBy(i => i.ExpValue)
                    .ToListAsync();

                _response.IsSucess = true;
                _response.Notification = "Get diamond items successfully";
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

        // YÊU CẦU 4: Lấy tất cả giao dịch của character, sắp xếp theo thời gian
        [HttpGet("Transactions/{characterId}")]
        public async Task<IActionResult> GetCharacterTransactions(byte characterId)
        {
            try
            {
                var character = await _db.Characters
                    .Include(c => c.Player)
                    .FirstOrDefaultAsync(c => c.CharacterId == characterId);

                if (character == null)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Character not found";
                    _response.Data = null;
                    return NotFound(_response);
                }

                var transactions = await _db.Transactions
                    .Where(t => t.CharacterId == characterId)
                    .Include(t => t.Item)
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(t => new
                    {
                        t.TransactionId,
                        t.TransactionDate,
                        t.Quantity,
                        t.TotalPrice,
                        t.PaymentMethod,
                        Item = new
                        {
                            t.Item.ItemId,
                            t.Item.ItemName,
                            t.Item.ItemType,
                            t.Item.Price
                        }
                    })
                    .ToListAsync();

                _response.IsSucess = true;
                _response.Notification = "Get transactions successfully";
                _response.Data = new
                {
                    Character = new
                    {
                        character.CharacterId,
                        character.CharacterName,
                        PlayerName = character.Player.PlayerName
                    },
                    TotalTransactions = transactions.Count,
                    TotalSpent = transactions.Sum(t => t.TotalPrice),
                    Transactions = transactions
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

        // YÊU CẦU 5: Thêm item mới
        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem([FromBody] AddItemDTO itemDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(itemDTO.ItemName))
                {
                    _response.IsSucess = false;
                    _response.Notification = "Item name is required";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                var item = new Item
                {
                    ItemName = itemDTO.ItemName,
                    ItemType = itemDTO.ItemType,
                    Price = itemDTO.Price,
                    ExpValue = itemDTO.ExpValue
                };

                _db.Items.Add(item);
                await _db.SaveChangesAsync();

                _response.IsSucess = true;
                _response.Notification = "Add item successfully";
                _response.Data = item;
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

        // YÊU CẦU 6: Cập nhật mật khẩu người chơi
        [HttpPut("UpdatePassword/{playerId}")]
        public async Task<IActionResult> UpdatePassword(byte playerId, [FromBody] UpdatePasswordDTO dto)
        {
            try
            {
                var player = await _db.Players.FindAsync(playerId);
                if (player == null)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Player not found";
                    _response.Data = null;
                    return NotFound(_response);
                }

                // Kiểm tra mật khẩu cũ nếu có
                if (!string.IsNullOrWhiteSpace(dto.OldPassword))
                {
                    var oldPasswordHash = HashPasswordToBytes(dto.OldPassword);
                    if (!player.PasswordHash.SequenceEqual(oldPasswordHash))
                    {
                        _response.IsSucess = false;
                        _response.Notification = "Old password is incorrect";
                        _response.Data = null;
                        return BadRequest(_response);
                    }
                }

                // Cập nhật mật khẩu mới
                player.PasswordHash = HashPasswordToBytes(dto.NewPassword);
                await _db.SaveChangesAsync();

                _response.IsSucess = true;
                _response.Notification = "Update password successfully";
                _response.Data = new
                {
                    player.PlayerId,
                    player.PlayerName
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

        // YÊU CẦU 7: Lấy danh sách items được mua nhiều nhất
        [HttpGet("MostPurchasedItems")]
        public async Task<IActionResult> GetMostPurchasedItems([FromQuery] int top = 10)
        {
            try
            {
                var result = await _db.Transactions
                    .GroupBy(t => new { t.ItemId, t.Item.ItemName, t.Item.ItemType })
                    .Select(g => new
                    {
                        ItemId = g.Key.ItemId,
                        ItemName = g.Key.ItemName,
                        ItemType = g.Key.ItemType,
                        TotalQuantity = g.Sum(t => t.Quantity),
                        TotalTransactions = g.Count(),
                        TotalRevenue = g.Sum(t => t.TotalPrice)
                    })
                    .OrderByDescending(x => x.TotalQuantity)
                    .Take(top)
                    .ToListAsync();

                _response.IsSucess = true;
                _response.Notification = "Get most purchased items successfully";
                _response.Data = result;
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
