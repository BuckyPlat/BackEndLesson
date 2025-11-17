using DataFirst.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataFirst.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerApiController : ControllerBase
    {
        private readonly GameVuiVlContext _context;

        public PlayerApiController(GameVuiVlContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlayers()
        {
            try
            {
                var players = await _context.Players.ToListAsync();
                return Ok(players);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayerById(int id)
        {
            try
            {
                var players = await _context.Players.FirstOrDefaultAsync(players => players.Id == id);
                return Ok(players);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayer(Player playerNew)
        {
            try
            {
                var players = await _context.Players.AddAsync(playerNew);
                await _context.SaveChangesAsync();
                return Ok(players);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
