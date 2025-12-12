using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterTankerManager.Data;
using WaterTankerManager.Models;

namespace WaterTankerManager.Controllers;

[Authorize]
public class RoundsController : Controller
{
    private readonly ApplicationDbContext _context;

    public RoundsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Rounds
    public async Task<IActionResult> Index(bool showHidden = false)
    {
        var rounds = _context.RoundEntries.AsQueryable();

        if (showHidden)
        {
            rounds = rounds.Where(r => r.IsHidden);
        }
        else
        {
            rounds = rounds.Where(r => !r.IsHidden);
        }

        ViewData["ShowHidden"] = showHidden;
        return View(await rounds.OrderByDescending(r => r.Date).ToListAsync());
    }

    // GET: Rounds/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new RoundEntry { Date = DateTime.Today });
    }

    // POST: Rounds/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("Id,Date,NumberOfRounds,TotalAmount")] RoundEntry roundEntry)
    {
        if (ModelState.IsValid)
        {
            _context.Add(roundEntry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(roundEntry);
    }

    // GET: Rounds/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var roundEntry = await _context.RoundEntries.FindAsync(id);
        if (roundEntry == null) return NotFound();
        return View(roundEntry);
    }

    // POST: Rounds/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Date,NumberOfRounds,TotalAmount")] RoundEntry roundEntry)
    {
        if (id != roundEntry.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(roundEntry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoundEntryExists(roundEntry.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(roundEntry);
    }

    // GET: Rounds/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var roundEntry = await _context.RoundEntries
            .FirstOrDefaultAsync(m => m.Id == id);
        if (roundEntry == null) return NotFound();

        return View(roundEntry);
    }

    // POST: Rounds/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var roundEntry = await _context.RoundEntries.FindAsync(id);
        if (roundEntry != null)
        {
            _context.RoundEntries.Remove(roundEntry);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: Rounds/ToggleStatus/5
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var roundEntry = await _context.RoundEntries.FindAsync(id);
        if (roundEntry == null)
        {
            return NotFound();
        }

        roundEntry.IsHidden = !roundEntry.IsHidden;
        _context.Update(roundEntry);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, isHidden = roundEntry.IsHidden });
    }

    private bool RoundEntryExists(int id)
    {
        return _context.RoundEntries.Any(e => e.Id == id);
    }
}
