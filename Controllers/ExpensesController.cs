using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaxiFleetManager.Data;
using TaxiFleetManager.Models;

namespace TaxiFleetManager.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expenses (Owner only)
        public async Task<IActionResult> Index()
        {
            var expenses = _context.Expenses
                .Include(e => e.Vehicle);
            return View(await expenses.ToListAsync());
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.Vehicle)
                .FirstOrDefaultAsync(m => m.ExpenseId == id);

            if (expense == null)
                return NotFound();

            return View(expense);
        }

        // GET: Expenses/Create
        public IActionResult Create()
        {
            // Show registration number in dropdown, not VehicleId number
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "RegistrationNumber");
            return View();
        }

        // POST: Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,ExpenseType,Amount,Description")] Expense expense)
        {
            // ExpenseDate has a DB default — set server-side
            ModelState.Remove("ExpenseDate");
            ModelState.Remove("Vehicle");

            if (ModelState.IsValid)
            {
                expense.ExpenseDate = DateTime.Now;
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "RegistrationNumber", expense.VehicleId);
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return NotFound();

            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "RegistrationNumber", expense.VehicleId);
            return View(expense);
        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpenseId,VehicleId,ExpenseType,Amount,ExpenseDate,Description")] Expense expense)
        {
            if (id != expense.ExpenseId)
                return NotFound();

            ModelState.Remove("Vehicle");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.ExpenseId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "RegistrationNumber", expense.VehicleId);
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.Vehicle)
                .FirstOrDefaultAsync(m => m.ExpenseId == id);

            if (expense == null)
                return NotFound();

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
                _context.Expenses.Remove(expense);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.ExpenseId == id);
        }
    }
}