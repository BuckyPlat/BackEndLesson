using Lab1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GambleController : ControllerBase
    {
        public IActionResult Get()
        {
            Gambler gambler = new Gambler
            {
                Id = 1,
                Name = "Lammao",
                Description = "POOR AF",
                Money = 10000,
            };
            int status = 1;
            string message = "Get data sucess!";
            var data = new { status, message, gambler };
            return new JsonResult(data);
        }
    }
}
