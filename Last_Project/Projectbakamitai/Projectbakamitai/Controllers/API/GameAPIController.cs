using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projectbakamitai.Data;
using Projectbakamitai.DTO;
using Projectbakamitai.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;



namespace Projectbakamitai.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameAPIController : ControllerBase
    {
        private readonly ProjectBakamitaiContext _context;
        protected ResponseAPI _response;
        private readonly IConfiguration _configuration;
        private byte[] HashPasswordToBytes(string password)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private readonly List<string> ItemTypeOrder = new()
        {
            "Supply",
            "Cat_Accessories",
            "Cat_Toys",
            "Terrain"
        };

        public GameAPIController(ProjectBakamitaiContext context,
            IConfiguration configuration)
        {
            _context = context;
            _response = new();
            _configuration = configuration;
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

                var hashed = HashPasswordToBytes(registerDTO.Password);

                var user = new User
                {
                    UserName = registerDTO.UserName,
                    Email = registerDTO.Email,
                    PasswordHash = hashed
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == loginDTO.Email);

                if (user == null)
                {
                    return BadRequest(new ResponseAPI
                    {
                        IsSuccess = false,
                        Notification = "Invalid email or password"
                    });
                }

                var hashed = HashPasswordToBytes(loginDTO.Password);

                if (!hashed.SequenceEqual(user.PasswordHash))
                {
                    return BadRequest(new ResponseAPI
                    {
                        IsSuccess = false,
                        Notification = "Invalid email or password"
                    });
                }

                var token = GenerateJwtToken(user);

                return Ok(new ResponseAPI
                {
                    IsSuccess = true,
                    Notification = "Login Successful",
                    Data = new
                    {
                        user.UserId,
                        user.UserName,
                        user.Email,
                        user.CreateAt,
                        token = token
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

        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            try
            {
                var profile = await _context.PlayerProfiles
                    .FirstOrDefaultAsync(p => p.UserId == userId);
                if (profile == null)
                {
                    return NotFound(new ResponseAPI
                    {
                        IsSuccess = false,
                        Notification = "Profile not found"
                    });
                }
                var dto = new PlayerProfileDTO
                {
                    ProfileId = profile.ProfileId,
                    UserId = profile.UserId,
                    DisplayName = profile.DisplayName,
                    AvatarUrl = profile.AvatarUrl,
                    Levels = profile.Levels ?? 1,
                    Exps = profile.Exps ?? 0,
                    Gold = profile.Gold ?? 0,
                    Gem = profile.Gem ?? 0
                };
                return Ok(new ResponseAPI
                {
                    IsSuccess = true,
                    Notification = "Profile retrieved successfully",
                    Data = dto
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

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new ResponseAPI
                    {
                        IsSuccess = false,
                        Notification = "User not found"
                    });
                }

                // Xóa profile
                var profile = await _context.PlayerProfiles
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile != null)
                    _context.PlayerProfiles.Remove(profile);

                // Xóa inventory
                var inventoryItems = _context.Inventories
                    .Where(i => i.UserId == userId);
                _context.Inventories.RemoveRange(inventoryItems);

                // Xóa giao dịch
                var transactions = _context.Transactions
                    .Where(t => t.UserId == userId);
                _context.Transactions.RemoveRange(transactions);

                // Cuối cùng xóa user
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new ResponseAPI
                {
                    IsSuccess = true,
                    Notification = "User and related data deleted successfully"
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

        [HttpPut("profile/{userId}")]
        public async Task<IActionResult> UpdateProfile(int userId, [FromForm] UpdateProfileDTO profileDTO)
        {
            try
            {
                var profile = await _context.PlayerProfiles
                    .FirstOrDefaultAsync(p => p.UserId == userId);
                if (profile == null)
                {
                    return NotFound(new ResponseAPI
                    {
                        IsSuccess = false,
                        Notification = "Profile not found"
                    });
                }
                if (!string.IsNullOrWhiteSpace(profileDTO.DisplayName))
                {
                    profile.DisplayName = profileDTO.DisplayName;
                }
                if (profileDTO.Avatar != null)
                {
                    var folder = Path.Combine("wwwroot", "images", "avatars");
                    Directory.CreateDirectory(folder);
                    string filename = Guid.NewGuid() + Path.GetExtension(profileDTO.Avatar.FileName);
                    string path = Path.Combine(folder, filename);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await profileDTO.Avatar.CopyToAsync(stream);
                    }
                    profile.AvatarUrl = "/images/avatars/" + filename;
                }
                await _context.SaveChangesAsync();
                var result = new PlayerProfileDTO
                {
                    ProfileId = profile.ProfileId,
                    UserId = profile.UserId,
                    DisplayName = profile.DisplayName,
                    AvatarUrl = profile.AvatarUrl,
                    Levels = profile.Levels ?? 1,
                    Exps = profile.Exps ?? 0,
                    Gold = profile.Gold ?? 0,
                    Gem = profile.Gem ?? 0
                };
                return Ok(new ResponseAPI
                {
                    IsSuccess = true,
                    Notification = "Profile updated successfully",
                    Data = result
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

        [HttpPost("addItem")]
        public async Task<IActionResult> AddItem([FromForm] AddItemsDTO itemsDTO)
        {
            try
            {
                var item = new Item
                {
                    ItemName = itemsDTO.ItemName,
                    Description = itemsDTO.Description,
                    PriceGold = itemsDTO.PriceGold,
                    PriceGem = itemsDTO.PriceGem,
                    ItemType = itemsDTO.ItemType,
                    IsShow = itemsDTO.IsShow ?? true
                };
                if (itemsDTO != null)
                {
                    var folder = Path.Combine("wwwroot", "images", "items");
                    Directory.CreateDirectory(folder);
                    string filename = Guid.NewGuid() + Path.GetExtension(itemsDTO.ProductImage.FileName);
                    string filepath = Path.Combine(folder, filename);
                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        await itemsDTO.ProductImage.CopyToAsync(stream);
                    }
                    item.ProductImage = "/images/items/" + filename;
                }
                _context.Items.Add(item);
                await _context.SaveChangesAsync();
                return Ok(new ResponseAPI
                {
                    IsSuccess = true,
                    Notification = "Item added successfully",
                    Data = item
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

        [HttpPost("addShop")]
        public async Task<IActionResult> AddShopProduct([FromBody] AddShopProductDTO dto)
        {
            try
            {
                // Validate input
                var item = await _context.Items.FindAsync(dto.ItemId);
                if (item == null)
                {
                    return NotFound(new ResponseAPI
                    {
                        IsSuccess = false,
                        Notification = "Item not found",
                        Data = null
                    });
                }

                // Try find existing shop product
                var existingShop = await _context.ShopProducts
                    .FirstOrDefaultAsync(s => s.ItemId == dto.ItemId);

                ShopProduct shopProduct;

                if (existingShop != null)
                {
                    shopProduct = existingShop;
                }
                else
                {
                    // Create new ShopProduct
                    shopProduct = new ShopProduct
                    {
                        ItemId = dto.ItemId,
                        IsAvailable = dto.IsAvailable
                    };

                    _context.ShopProducts.Add(shopProduct);
                    await _context.SaveChangesAsync();
                }

                // Return a simple DTO (do not return EF entity directly to avoid cycles)
                var result = new
                {
                    shopProduct.ShopItemId,
                    shopProduct.ItemId,
                    shopProduct.IsAvailable
                };

                return Ok(new ResponseAPI
                {
                    IsSuccess = true,
                    Notification = existingShop != null
                        ? "Shop product already exists"
                        : "Shop product added successfully",
                    Data = result
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

        [HttpGet("ShopProduct")]
        public async Task<IActionResult> GetShopItems([FromQuery] bool onlyAvailable = true)
        {
            try
            {
                var query = _context.ShopProducts
                    .Include(sp => sp.Item)
                    .AsQueryable();
                if (onlyAvailable)
                {
                    query = query.Where(sp => sp.IsAvailable == true);
                }
                var result = await query
                    .Select(sp => new ShopItemDTO
                    {
                        ShopItemId = sp.ShopItemId,
                        ItemId = sp.Item.ItemId,
                        ItemName = sp.Item.ItemName,
                        Description = sp.Item.Description,
                        ProductImage = sp.Item.ProductImage,
                        PriceGold = sp.Item.PriceGold,
                        PriceGem = sp.Item.PriceGem,
                        ItemType = sp.Item.ItemType,
                        IsAvailable = sp.IsAvailable
                    })
                    .ToListAsync();
                result = result
                    .OrderBy(x => ItemTypeOrder.IndexOf(x.ItemType))
                    .ToList();
                return Ok(new ResponseAPI
                {
                    IsSuccess = true,
                    Notification = "Shop items retrieved successfully",
                    Data = result
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

        [HttpPost("buy")]
        public async Task<IActionResult> BuyItem([FromBody] BuyItemDTO dto)
        {
            try
            {
                var user = _context.PlayerProfiles.FirstOrDefault(p => p.UserId == dto.UserId);
                var item = _context.Items.FirstOrDefault(i => i.ItemId == dto.ItemId);

                if (user == null || item == null)
                {
                    return BadRequest(new ResponseAPI
                    {
                        IsSuccess = false,
                        Notification = "Invalid user or item",
                        Data = null
                    });
                }

                if (user.Gold < item.PriceGold || user.Gem < item.PriceGem)
                {
                    return BadRequest(new ResponseAPI
                    {
                        IsSuccess = false,
                        Notification = "Not enough currency",
                        Data = new
                        {
                            RequiredGold = item.PriceGold,
                            RequiredGem = item.PriceGem,
                            UserGold = user.Gold,
                            UserGem = user.Gem
                        }
                    });
                }

                user.Gold -= item.PriceGold ?? 0;
                user.Gem -= item.PriceGem ?? 0;

                var inv = _context.Inventories
                    .FirstOrDefault(i => i.UserId == dto.UserId && i.ItemId == dto.ItemId);

                if (inv != null)
                {
                    inv.Quantity = (inv.Quantity ?? 0) + 1;
                }
                else
                {
                    inv = new Inventory
                    {
                        UserId = dto.UserId,
                        ItemId = dto.ItemId,
                        Quantity = 1,
                        PurchasePriceGold = item.PriceGold,
                        PurchasePriceGem = item.PriceGem,
                        PurchasedAt = DateTime.UtcNow
                    };
                    _context.Inventories.Add(inv);
                }

                var transaction = new Transaction
                {
                    UserId = dto.UserId,
                    ItemId = dto.ItemId,
                    TransactionType = "Buy",
                    CurrencyType = "Gold+Gem",
                    Amount = (item.PriceGold ?? 0) + (item.PriceGem ?? 0),
                    Quantity = 1,
                    CreateAt = DateTime.UtcNow
                };

                _context.Transactions.Add(transaction);

                await _context.SaveChangesAsync();

                return Ok(new ResponseAPI
                {
                    IsSuccess = true,
                    Notification = "Purchase success",
                    Data = new
                    {
                        User = new { user.UserId, user.Gold, user.Gem },
                        Inventory = new { inv.InvenId, inv.ItemId, inv.Quantity },
                        Transaction = new
                        {
                            transaction.TransactionId,
                            transaction.TransactionType,
                            transaction.Amount,
                            transaction.CurrencyType,
                            transaction.CreateAt
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI
                {
                    IsSuccess = false,
                    Notification = "Error",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("inventory/{userId}")]
        public async Task<IActionResult> GetInventoryByUserId(int userId)
        {
            try
            {
                var userExists = _context.Users.Any(u => u.UserId == userId);
                if (!userExists)
                {
                    return NotFound(new ResponseAPI
                    {
                        IsSuccess = false,
                        Notification = "User not found",
                        Data = null
                    });
                }

                var inventory = await _context.Inventories
                    .Where(i => i.UserId == userId)
                    .Select(i => new InventoryDTO
                    {
                        InvenId = i.InvenId,
                        ItemId = i.ItemId,
                        ItemName = i.Item.ItemName,
                        Description = i.Item.Description,
                        ProductImage = i.Item.ProductImage,
                        ItemType = i.Item.ItemType,
                        Quantity = i.Quantity ?? 1,
                        PurchasePriceGold = i.PurchasePriceGold,
                        PurchasePriceGem = i.PurchasePriceGem,
                        PurchasedAt = i.PurchasedAt
                    })
                    .ToListAsync();

                return Ok(new ResponseAPI
                {
                    IsSuccess = true,
                    Notification = "Inventory retrieved successfully",
                    Data = inventory
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
