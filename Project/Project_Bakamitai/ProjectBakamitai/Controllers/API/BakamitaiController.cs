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

        private static readonly Dictionary<string, byte> TypeToShop = new()
        {
            { "Transportation", 1 },
            { "Weapon", 2 },
            { "Tool", 3 },
            { "Resource", 4 }
        };


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
                var items = await _db.Items
                    .Include(i => i.ShopItems)
                    .ThenInclude(si => si.Shop)
                    .Select(i => new
                    {
                        ItemId = i.ItemId,
                        ItemName = i.ItemName,
                        ItemType = i.ItemType,
                        Price = i.Price,
                        Shops = i.ShopItems
                            .Select(si => si.Shop.ShopName)
                            .ToList()
                    })
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

        [HttpGet("GetItemsByType/{type}")]
        public async Task<IActionResult> GetItemsByType(string type)
        {
            try
            {
                var items = await _db.Items
                    .Where(i => i.ItemType == type)
                    .Include(i => i.ShopItems)
                    .ThenInclude(si => si.Shop)
                    .Select(i => new
                    {
                        ItemId = i.ItemId,
                        ItemName = i.ItemName,
                        ItemType = i.ItemType,
                        Price = i.Price,
                        Shops = i.ShopItems
                            .Select(si => si.Shop.ShopName)
                            .ToList()
                    })
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
                    .Where(i => i.Price < 200)
                    .Include(i => i.ShopItems)
                    .ThenInclude(si => si.Shop)
                    .Select(i => new
                    {
                        ItemId = i.ItemId,
                        ItemName = i.ItemName,
                        ItemType = i.ItemType,
                        Price = i.Price,
                        Shops = i.ShopItems
                            .Select(si => si.Shop.ShopName)
                            .ToList()
                    })
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
            var response = new ResponseApi();
            try
            {
                if (string.IsNullOrWhiteSpace(itemDTO.ItemName))
                {
                    response.IsSucess = false;
                    response.Notification = "Item name is required";
                    return BadRequest(response);
                }

                if (string.IsNullOrWhiteSpace(itemDTO.ItemType))
                {
                    response.IsSucess = false;
                    response.Notification = "Item type is required";
                    return BadRequest(response);
                }

                if (itemDTO.Price <= 0)
                {
                    response.IsSucess = false;
                    response.Notification = "Price must be greater than zero";
                    return BadRequest(response);
                }
                var typeToShop = new Dictionary<string, byte>
                {
                    { "Transportation", 1 },
                    { "Weapon", 2 },
                    { "Tool", 3 },
                    { "Resource", 4 }
                };

                var item = new Item
                {
                    ItemName = itemDTO.ItemName,
                    ItemType = itemDTO.ItemType,
                    Price = itemDTO.Price
                };

                _db.Items.Add(item);
                await _db.SaveChangesAsync();
                if (typeToShop.TryGetValue(item.ItemType, out byte shopId))
                {
                    var shopItem = new ShopItem
                    {
                        ShopId = shopId,
                        ItemId = item.ItemId
                    };

                    _db.ShopItems.Add(shopItem);
                    await _db.SaveChangesAsync();
                }

                response.IsSucess = true;
                response.Notification = "Item added successfully";
                response.Data = new
                {
                    item.ItemId,
                    item.ItemName,
                    item.ItemType,
                    item.Price,
                    AssignedShop = typeToShop.ContainsKey(item.ItemType)
                                    ? (byte?)typeToShop[item.ItemType]
                                    : null
                };


                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSucess = false;
                response.Notification = "Error";
                response.Data = ex.Message;
                return BadRequest(response);
            }
        }



        // YÊU CẦU 8: Cập nhật mật khẩu người chơi
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePasswordByEmail([FromBody] UpdatePasswordDTO updatepasswordDTO)
        {
            try
            {
                var player = await _db.Players
                    .FirstOrDefaultAsync(p => p.Email == updatepasswordDTO.Email);

                if (player == null)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Email not found";
                    return NotFound(_response);
                }

                var oldHash = HashPasswordToBytes(updatepasswordDTO.OldPassword);

                if (!player.PasswordHash.SequenceEqual(oldHash))
                {
                    _response.IsSucess = false;
                    _response.Notification = "Old password is incorrect";
                    return BadRequest(_response);
                }

                player.PasswordHash = HashPasswordToBytes(updatepasswordDTO.NewPassword);
                await _db.SaveChangesAsync();

                _response.IsSucess = true;
                _response.Notification = "Password updated successfully";
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

        [HttpDelete("DeleteItem/{id}")]
        public async Task<IActionResult> DeleteItem(byte id)
        {
            var response = new ResponseApi();

            try
            {
                var item = await _db.Items
                    .Include(i => i.ShopItems)
                    .FirstOrDefaultAsync(i => i.ItemId == id);
                if (item == null)
                {
                    response.IsSucess = false;
                    response.Notification = "Item not found";
                    return NotFound(response);
                }
                if (item.ShopItems != null && item.ShopItems.Any())
                {
                    _db.ShopItems.RemoveRange(item.ShopItems);
                }
                _db.Items.Remove(item);
                await _db.SaveChangesAsync();
                response.IsSucess = true;
                response.Notification = "Item deleted successfully";
                response.Data = new
                {
                    DeletedItemId = id
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSucess = false;
                response.Notification = "Error";
                response.Data = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("Buy")]
        public async Task<IActionResult> BuyItem([FromBody] BuyItemDTO dto)
        {
            var response = new ResponseApi();

            try
            {
                // 1. Check Player
                var player = await _db.Players
                    .Include(p => p.Characters)
                    .FirstOrDefaultAsync(p => p.PlayerId == dto.PlayerId);

                if (player == null)
                {
                    response.IsSucess = false;
                    response.Notification = "Player not found";
                    return NotFound(response);
                }

                // 2. Get Character (only 1)
                var character = player.Characters.FirstOrDefault();
                if (character == null)
                {
                    response.IsSucess = false;
                    response.Notification = "Player has no character";
                    return BadRequest(response);
                }

                // 3. Check Item
                var item = await _db.Items.FirstOrDefaultAsync(i => i.ItemId == dto.ItemId);
                if (item == null)
                {
                    response.IsSucess = false;
                    response.Notification = "Item not found";
                    return BadRequest(response);
                }

                // 4. Check shop sells item
                bool isSold = await _db.ShopItems
                    .AnyAsync(si => si.ShopId == dto.ShopId && si.ItemId == dto.ItemId);

                if (!isSold)
                {
                    response.IsSucess = false;
                    response.Notification = "Shop does not sell this item";
                    return BadRequest(response);
                }

                // 5. Transportation can only be bought once
                if (item.ItemType == "Transportation")
                {
                    dto.Quantity = 1;

                    bool alreadyOwned = await _db.Inventories
                        .AnyAsync(i => i.CharacterId == character.CharacterId && i.ItemId == item.ItemId);

                    if (alreadyOwned)
                    {
                        response.IsSucess = false;
                        response.Notification = "You already own this transportation item";
                        return BadRequest(response);
                    }
                }

                // 6. Enough gold?
                int total = item.Price * dto.Quantity;
                if (character.Gold < total)
                {
                    response.IsSucess = false;
                    response.Notification = "Not enough gold";
                    return BadRequest(response);
                }

                character.Gold -= total;

                // 7. Inventory update
                var inv = await _db.Inventories
                    .FirstOrDefaultAsync(i => i.CharacterId == character.CharacterId && i.ItemId == item.ItemId);

                if (inv == null)
                {
                    inv = new Inventory
                    {
                        CharacterId = character.CharacterId,
                        ItemId = item.ItemId,
                        Quantity = dto.Quantity,
                        AcquireDate = DateTime.Now
                    };
                    _db.Inventories.Add(inv);
                }
                else
                {
                    inv.Quantity += dto.Quantity;
                }

                // 8. Create transaction log
                var transaction = new Transaction
                {
                    CharacterId = character.CharacterId,
                    ShopId = dto.ShopId,
                    ItemId = dto.ItemId,
                    Quantity = dto.Quantity,
                    TotalPrice = total,
                    PaymentMethod = "Gold",
                    TransactionDate = DateTime.Now
                };

                _db.Transactions.Add(transaction);
                await _db.SaveChangesAsync();

                // 9. Response
                response.IsSucess = true;
                response.Notification = "Purchase completed";
                response.Data = new
                {
                    CharacterName = character.CharacterName,
                    ItemName = item.ItemName,
                    Bought = dto.Quantity,
                    RemainingGold = character.Gold,
                    InventoryQuantity = inv.Quantity
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSucess = false;
                response.Notification = "Error";
                response.Data = ex.Message;
                return BadRequest(response);
            }
        }


        // YÊU CẦU 6: Lấy tất cả giao dịch mua item và phương tiện của một người chơi cụ thể,
        [HttpGet("PlayerAllTransactions/{playerId}")]
        public async Task<IActionResult> GetPlayerAllTransactions(byte playerId)
        {
            try
            {
                var player = await _db.Players
                    .Include(p => p.Characters)
                    .FirstOrDefaultAsync(p => p.PlayerId == playerId);

                if (player == null)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Player not found";
                    _response.Data = null;
                    return NotFound(_response);
                }

                var characterIds = player.Characters.Select(c => c.CharacterId).ToList();

                var transactions = await _db.Transactions
                    .Where(t => characterIds.Contains(t.CharacterId))
                    .Include(t => t.Item)
                    .Include(t => t.Character)
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(t => new
                    {
                        t.TransactionId,
                        t.TransactionDate,
                        t.Quantity,
                        t.TotalPrice,
                        t.PaymentMethod,
                        Character = new
                        {
                            t.Character.CharacterId,
                            t.Character.CharacterName
                        },
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
                _response.Notification = "Get all player transactions successfully";
                _response.Data = new
                {
                    Player = new
                    {
                        player.PlayerId,
                        player.PlayerName,
                        player.Email
                    },
                    TotalCharacters = player.Characters.Count,
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

        // YÊU CẦU 9: Lấy danh sách các item được mua nhiều nhất
        [HttpGet("TopPurchasedItems")]
        public async Task<IActionResult> GetTopPurchasedItems([FromQuery] int top = 10)
        {
            try
            {
                var topItems = await _db.Transactions
                    .GroupBy(t => new
                    {
                        t.ItemId,
                        t.Item.ItemName,
                        t.Item.ItemType,
                        t.Item.Price
                    })
                    .Select(g => new
                    {
                        ItemId = g.Key.ItemId,
                        ItemName = g.Key.ItemName,
                        ItemType = g.Key.ItemType,
                        ItemPrice = g.Key.Price,
                        TotalQuantitySold = g.Sum(t => t.Quantity),
                        TotalTransactions = g.Count(),
                        TotalRevenue = g.Sum(t => t.TotalPrice),
                        UniqueCustomers = g.Select(t => t.CharacterId).Distinct().Count()
                    })
                    .OrderByDescending(x => x.TotalQuantitySold)
                    .Take(top)
                    .ToListAsync();

                _response.IsSucess = true;
                _response.Notification = "Get top purchased items successfully";
                _response.Data = new
                {
                    Top = top,
                    TotalItems = topItems.Count,
                    Items = topItems
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

        // YÊU CẦU 10: Lấy danh sách tất cả người chơi và số lần họ đã mua hàng
        [HttpGet("PlayersWithPurchaseCount")]
        public async Task<IActionResult> GetPlayersWithPurchaseCount()
        {
            try
            {
                var playersWithStats = await _db.Players
                    .Select(p => new
                    {
                        p.PlayerId,
                        p.PlayerName,
                        p.Email,
                        p.CreateDate,
                        TotalCharacters = p.Characters.Count,
                        TotalPurchases = p.Characters
                            .SelectMany(c => c.Transactions)
                            .Count(),
                        TotalItemsBought = p.Characters
                            .SelectMany(c => c.Transactions)
                            .Sum(t => (int?)t.Quantity) ?? 0,
                        TotalMoneySpent = p.Characters
                            .SelectMany(c => c.Transactions)
                            .Sum(t => (int?)t.TotalPrice) ?? 0,
                        LastPurchaseDate = p.Characters
                            .SelectMany(c => c.Transactions)
                            .OrderByDescending(t => t.TransactionDate)
                            .Select(t => (DateTime?)t.TransactionDate)
                            .FirstOrDefault(),
                        MostBoughtItem = p.Characters
                            .SelectMany(c => c.Transactions)
                            .GroupBy(t => t.Item.ItemName)
                            .OrderByDescending(g => g.Sum(t => t.Quantity))
                            .Select(g => g.Key)
                            .FirstOrDefault()
                    })
                    .OrderByDescending(p => p.TotalPurchases)
                    .ToListAsync();

                var summary = new
                {
                    TotalPlayers = playersWithStats.Count,
                    TotalPlayersWithPurchases = playersWithStats.Count(p => p.TotalPurchases > 0),
                    TotalPlayersWithoutPurchases = playersWithStats.Count(p => p.TotalPurchases == 0),
                    AveragePurchasesPerPlayer = playersWithStats.Any()
                        ? playersWithStats.Average(p => p.TotalPurchases)
                        : 0,
                    TotalRevenue = playersWithStats.Sum(p => p.TotalMoneySpent)
                };

                _response.IsSucess = true;
                _response.Notification = "Get all players with purchase statistics successfully";
                _response.Data = new
                {
                    Summary = summary,
                    Players = playersWithStats
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

        [HttpGet("PlayerDetailedStats/{playerId}")]
        public async Task<IActionResult> GetPlayerDetailedStats(byte playerId)
        {
            try
            {
                var player = await _db.Players
                    .Include(p => p.Characters)
                        .ThenInclude(c => c.Transactions)
                            .ThenInclude(t => t.Item)
                    .FirstOrDefaultAsync(p => p.PlayerId == playerId);

                if (player == null)
                {
                    _response.IsSucess = false;
                    _response.Notification = "Player not found";
                    _response.Data = null;
                    return NotFound(_response);
                }

                var allTransactions = player.Characters
                    .SelectMany(c => c.Transactions)
                    .ToList();

                var characterStats = player.Characters.Select(c => new
                {
                    c.CharacterId,
                    c.CharacterName,
                    c.Experience,
                    c.Gold,
                    TotalPurchases = c.Transactions.Count,
                    TotalSpent = c.Transactions.Sum(t => t.TotalPrice),
                    MostBoughtItem = c.Transactions
                        .GroupBy(t => t.Item.ItemName)
                        .OrderByDescending(g => g.Sum(t => t.Quantity))
                        .Select(g => g.Key)
                        .FirstOrDefault()
                }).ToList();

                var itemTypeDistribution = allTransactions
                    .GroupBy(t => t.Item.ItemType)
                    .Select(g => new
                    {
                        ItemType = g.Key,
                        Count = g.Count(),
                        TotalSpent = g.Sum(t => t.TotalPrice)
                    })
                    .ToList();

                var paymentMethodDistribution = allTransactions
                    .GroupBy(t => t.PaymentMethod)
                    .Select(g => new
                    {
                        PaymentMethod = g.Key,
                        Count = g.Count(),
                        TotalAmount = g.Sum(t => t.TotalPrice)
                    })
                    .ToList();

                _response.IsSucess = true;
                _response.Notification = "Get player detailed statistics successfully";
                _response.Data = new
                {
                    Player = new
                    {
                        player.PlayerId,
                        player.PlayerName,
                        player.Email,
                        player.CreateDate
                    },
                    OverallStats = new
                    {
                        TotalCharacters = player.Characters.Count,
                        TotalPurchases = allTransactions.Count,
                        TotalSpent = allTransactions.Sum(t => t.TotalPrice),
                        TotalItemsBought = allTransactions.Sum(t => t.Quantity),
                        FirstPurchase = allTransactions.OrderBy(t => t.TransactionDate).FirstOrDefault()?.TransactionDate,
                        LastPurchase = allTransactions.OrderByDescending(t => t.TransactionDate).FirstOrDefault()?.TransactionDate
                    },
                    Characters = characterStats,
                    ItemTypeDistribution = itemTypeDistribution,
                    PaymentMethodDistribution = paymentMethodDistribution,
                    RecentTransactions = allTransactions
                        .OrderByDescending(t => t.TransactionDate)
                        .Take(5)
                        .Select(t => new
                        {
                            t.TransactionId,
                            t.TransactionDate,
                            CharacterName = t.Character.CharacterName,
                            ItemName = t.Item.ItemName,
                            t.Quantity,
                            t.TotalPrice,
                            t.PaymentMethod
                        })
                        .ToList()
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
    }
}