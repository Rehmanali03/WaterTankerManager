using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterTankerManager.Data;
using WaterTankerManager.Models;

namespace WaterTankerManager.Controllers;

[Authorize]
public class ExpensesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ExpensesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Expenses
    public async Task<IActionResult> Index()
    {
        return View(await _context.ExpenseEntries.OrderByDescending(e => e.Date).ToListAsync());
    }

    // GET: Expenses/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new ExpenseEntry { Date = DateTime.Today });
    }

    // POST: Expenses/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("Id,Date,ExpenseName,ExpensePrice")] ExpenseEntry expenseEntry)
    {
        if (ModelState.IsValid)
        {
            _context.Add(expenseEntry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(expenseEntry);
    }

    // GET: Expenses/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var expenseEntry = await _context.ExpenseEntries.FindAsync(id);
        if (expenseEntry == null) return NotFound();
        return View(expenseEntry);
    }

    // POST: Expenses/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Date,ExpenseName,ExpensePrice")] ExpenseEntry expenseEntry)
    {
        if (id != expenseEntry.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(expenseEntry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseEntryExists(expenseEntry.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(expenseEntry);
    }

    // GET: Expenses/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var expenseEntry = await _context.ExpenseEntries
            .FirstOrDefaultAsync(m => m.Id == id);
        if (expenseEntry == null) return NotFound();

        return View(expenseEntry);
    }

    // POST: Expenses/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var expenseEntry = await _context.ExpenseEntries.FindAsync(id);
        if (expenseEntry != null)
        {
            _context.ExpenseEntries.Remove(expenseEntry);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ExpenseEntryExists(int id)
    {
        return _context.ExpenseEntries.Any(e => e.Id == id);
    }
}
