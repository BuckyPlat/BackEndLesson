using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab2.Data;
using Lab2.Models;

namespace Lab2.Controllers
{
    public class GameLevelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameLevelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GameLevels
        public async Task<IActionResult> Index()
        {
            return View(await _context.GameLevels.ToListAsync());
        }

        // GET: GameLevels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameLevel = await _context.GameLevels
                .FirstOrDefaultAsync(m => m.LevelId == id);
            if (gameLevel == null)
            {
                return NotFound();
            }

            return View(gameLevel);
        }

        // GET: GameLevels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GameLevels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LevelId,title,description")] GameLevel gameLevel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gameLevel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gameLevel);
        }

        // GET: GameLevels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameLevel = await _context.GameLevels.FindAsync(id);
            if (gameLevel == null)
            {
                return NotFound();
            }
            return View(gameLevel);
        }

        // POST: GameLevels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LevelId,title,description")] GameLevel gameLevel)
        {
            if (id != gameLevel.LevelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameLevelExists(gameLevel.LevelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gameLevel);
        }

        // GET: GameLevels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameLevel = await _context.GameLevels
                .FirstOrDefaultAsync(m => m.LevelId == id);
            if (gameLevel == null)
            {
                return NotFound();
            }

            return View(gameLevel);
        }

        // POST: GameLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameLevel = await _context.GameLevels.FindAsync(id);
            if (gameLevel != null)
            {
                _context.GameLevels.Remove(gameLevel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameLevelExists(int id)
        {
            return _context.GameLevels.Any(e => e.LevelId == id);
        }
    }
}
