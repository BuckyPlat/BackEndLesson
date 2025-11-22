using Lab2.Data;
using Lab2.DTO;
using Lab2.Models;
using Lab2.Views.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIGameController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        protected ResponseApi _response;
        private readonly UserManager<ApplicationUser> _userManager;

        public APIGameController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _response = new();
            _userManager = userManager;
        }
        [HttpGet("GetAllGameLevels")]
        public async Task<IActionResult> GetAllGameLevel()
        {
            try
            {
                var gameLevel = await _db.GameLevels.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lấy dữ liệu thành công";
                _response.Data = gameLevel;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpGet("GetAllQuestionsGame")]
        public async Task<IActionResult> GetAllQuestionGame()
        {
            try
            {
                var QuestionGame = await _db.Questions.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lấy dữ liệu thành công";
                _response.Data = QuestionGame;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpGet("GetAllRegions")]
        public async Task<IActionResult> GetAllRegions()
        {
            try
            {
                var Regions = await _db.Regions.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lấy dữ liệu thành công";
                _response.Data = Regions;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTOcs registerDTO)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Email = registerDTO.Email,
                    UserName = registerDTO.Email,
                    Name = registerDTO.Name,
                    Avatar = registerDTO.LinkAvatar,
                    RegionId = registerDTO.RegionId
                };
                var result = await _userManager.CreateAsync(user, registerDTO.Password);
                if (result.Succeeded)
                {
                    _response.IsSuccess = true;
                    _response.Notification = "Đăng ký thành công";
                    _response.Data = user;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Đăng ký thất bại";
                    _response.Data = result.Errors;
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var email = loginRequest.Email;
                var password = loginRequest.Password;

                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    _response.IsSuccess = true;
                    _response.Notification = "Đăng nhập thành công";
                    _response.Data = user;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Đăng nhập thất bại";
                    _response.Data = "Email hoặc mật khẩu không đúng";
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpGet("GetAllQuestionGameByLevel/{LevelId}")]
        public async Task<IActionResult> GetAllQuestionGameByLevel(int LevelId)
        {
            try
            {
                var questionGame = await _db.Questions.Where(x => x.LevelId == LevelId).ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lấy dữ liệu thành công";
                _response.Data = questionGame;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpPost("SaveResult")]
        public async Task<IActionResult> SaveResult(LevelResultDTO levelResult)
        {
            try
            {
                var levelResultSave = new LevelResult
                {
                    UserId = levelResult.UserId,
                    LevelId = levelResult.LevelId,
                    Score = levelResult.Score,
                    CompletionDate = DateOnly.FromDateTime(DateTime.Now)
                };

                await _db.LevelResults.AddAsync(levelResultSave);
                await _db.SaveChangesAsync();

                _response.IsSuccess = true;
                _response.Notification = "Lưu kết quả thành công";
                _response.Data = levelResult;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpGet("Rating/{idRegion}")]
        public async Task<IActionResult> Rating(int idRegion)
        {
            try
            {
                if (idRegion > 0)
                {
                    var nameRegion = await _db.Regions.Where(x => x.RegionId == idRegion)
                                                      .Select(x => x.Name)
                                                      .FirstOrDefaultAsync();
                    if (nameRegion == null)
                    {
                        _response.IsSuccess = false;
                        _response.Notification = "Không tìm thấy khu vực";
                        _response.Data = null;
                        return BadRequest(_response);
                    }

                    var userByRegion = await _db.Users.Where(x => x.RegionId == idRegion).ToListAsync();
                    var resultLevelByRegion = await _db.LevelResults
                                                       .Where(x => userByRegion.Select(t => t.Id)
                                                       .Contains(x.UserId))
                                                       .ToListAsync();

                    RatingVM ratingVM = new();
                    ratingVM.NameRegion = nameRegion;
                    ratingVM.userResultSums = new();

                    foreach (var item in userByRegion)
                    {
                        var sumScore = resultLevelByRegion
                                       .Where(x => x.UserId == item.Id)
                                       .Sum(x => x.Score);

                        var sumLevel = resultLevelByRegion
                                       .Where(x => x.UserId == item.Id)
                                       .Count();

                        UserResultSum userResultSum = new();
                        userResultSum.NameUser = item.Name;
                        userResultSum.SumScore = sumScore;
                        userResultSum.SumLevel = sumLevel;
                        ratingVM.userResultSums.Add(userResultSum);
                    }

                    _response.IsSuccess = true;
                    _response.Notification = "Lấy dữ liệu thành công";
                    _response.Data = ratingVM;
                    return Ok(_response);
                }
                else
                {
                    var user = await _db.Users.ToListAsync();
                    var resultLevel = await _db.LevelResults.ToListAsync();
                    string nameRegion = "Tất cả";

                    RatingVM ratingVM = new();
                    ratingVM.NameRegion = nameRegion;
                    ratingVM.userResultSums = new();

                    foreach (var item in user)
                    {
                        var sumScore = resultLevel
                                       .Where(x => x.UserId == item.Id)
                                       .Sum(x => x.Score);

                        var sumLevel = resultLevel
                                       .Where(x => x.UserId == item.Id)
                                       .Count();

                        UserResultSum userResultSum = new();
                        userResultSum.NameUser = item.Name;
                        userResultSum.SumScore = sumScore;
                        userResultSum.SumLevel = sumLevel;
                        ratingVM.userResultSums.Add(userResultSum);
                    }

                    _response.IsSuccess = true;
                    _response.Notification = "Lấy dữ liệu thành công";
                    _response.Data = ratingVM;
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi!";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpGet("GetUserInformation/{userId}")]
        public async Task<IActionResult> GetUserInformation(string userId)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }
                UserInformationVM userInformationVM = new();
                userInformationVM.Name = user.Name;
                userInformationVM.Email = user.Email;
                userInformationVM.Avartar = user.Avatar;
                userInformationVM.Region = await _db.Regions.Where(x => x.RegionId == user.RegionId)
                    .Select(x => x.Name).FirstOrDefaultAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lấy dữ liệu thành công";
                _response.Data = userInformationVM;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGameLevel(int id, GameLevel gameLevel)
        {
            try
            {
                if (id != gameLevel.LevelId)
                {
                    return BadRequest();
                }
                _db.Update(gameLevel);
                await _db.SaveChangesAsync();
                return Ok(gameLevel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGameLevel(int id)
        {
            try
            {
                var gameLevel = await _db.GameLevels.FindAsync(id);
                if (gameLevel == null)
                {
                    return NotFound();
                }
                _db.GameLevels.Remove(gameLevel);
                await _db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("uploadsingle")]
        public async Task<IActionResult> UploadSingle([FromForm] FormFileData file)
        {
            try
            {
                var fileExtension = Path.GetExtension(file.formFile.FileName);
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.formFile.CopyToAsync(stream);
                }
                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                return Ok(new { fileUrl });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("uploadmultiple")]
        public async Task<IActionResult> UploadMultiple([FromForm] ListFormFileData files)
        {
            try
            {
                List<string> fileUrls = new();

                foreach (var file in files.formFiles)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = Guid.NewGuid().ToString() + fileExtension;

                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                    fileUrls.Add(fileUrl);
                }

                return Ok(new { fileUrls });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

