using DataFirst.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataFirst.Controllers
{
    public class PlayerController : Controller
    {
        private readonly GameVuiVlContext _context;

        public PlayerController(GameVuiVlContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var players = _context.Players.ToList();
            return View(players);
        }
    }
}
