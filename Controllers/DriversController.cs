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
    public class DriversController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DriversController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Drivers (Owner only)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Drivers.ToListAsync());
        }

        // GET: Drivers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var driver = await _context.Drivers
                .Include(d => d.Vehicles)
                .Include(d => d.Trips)
                .FirstOrDefaultAsync(m => m.DriverId == id);

            if (driver == null)
                return NotFound();

            return View(driver);
        }

        // GET: Drivers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Drivers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,ContactNumber,LicenseNumber,Status,Pin")] Driver driver)
        {
            // HireDate has a DB default (GETDATE()) — remove from validation
            ModelState.Remove("HireDate");
            // Trips and Vehicles are navigation collections — not submitted by form
            ModelState.Remove("Trips");
            ModelState.Remove("Vehicles");

            if (ModelState.IsValid)
            {
                driver.HireDate = DateOnly.FromDateTime(DateTime.Today);
                _context.Add(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(driver);
        }

        // GET: Drivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
                return NotFound();

            return View(driver);
        }

        // POST: Drivers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DriverId,FullName,ContactNumber,LicenseNumber,HireDate,Status,Pin")] Driver driver)
        {
            if (id != driver.DriverId)
                return NotFound();

            ModelState.Remove("Trips");
            ModelState.Remove("Vehicles");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.DriverId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(driver);
        }

        // GET: Drivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(m => m.DriverId == id);

            if (driver == null)
                return NotFound();

            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver != null)
                _context.Drivers.Remove(driver);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.DriverId == id);
        }
    }
}