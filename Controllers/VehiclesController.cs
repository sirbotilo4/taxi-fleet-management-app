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
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehicles (Owner only — full fleet)
        public async Task<IActionResult> Index()
        {
            var vehicles = _context.Vehicles
                .Include(v => v.Driver);
            return View(await vehicles.ToListAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var vehicle = await _context.Vehicles
                .Include(v => v.Driver)
                .Include(v => v.Trips)
                    .ThenInclude(t => t.Route)
                .Include(v => v.Trips)
                    .ThenInclude(t => t.Driver)
                .Include(v => v.Expenses)
                .FirstOrDefaultAsync(m => m.VehicleId == id);

            if (vehicle == null)
                return NotFound();

            // Pass computed stats to view
            ViewBag.TotalRevenue = vehicle.Trips.Sum(t => t.AmountCollected);
            ViewBag.TotalExpenses = vehicle.Expenses.Sum(e => e.Amount);
            ViewBag.NetProfit = ViewBag.TotalRevenue - ViewBag.TotalExpenses;
            ViewBag.TotalTrips = vehicle.Trips.Count;

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            // Show driver name in dropdown, not DriverId number
            ViewData["DriverId"] = new SelectList(_context.Drivers.Where(d => d.Status == "Active"), "DriverId", "FullName");
            return View();
        }

        // POST: Vehicles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistrationNumber,Make,Model,Capacity,Status,DriverId")] Vehicle vehicle)
        {
            ModelState.Remove("Driver");
            ModelState.Remove("Trips");
            ModelState.Remove("Expenses");

            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DriverId"] = new SelectList(_context.Drivers.Where(d => d.Status == "Active"), "DriverId", "FullName", vehicle.DriverId);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return NotFound();

            // Show all drivers in edit (including inactive — owner may want to reassign)
            ViewData["DriverId"] = new SelectList(_context.Drivers, "DriverId", "FullName", vehicle.DriverId);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VehicleId,RegistrationNumber,Make,Model,Capacity,Status,DriverId")] Vehicle vehicle)
        {
            if (id != vehicle.VehicleId)
                return NotFound();

            ModelState.Remove("Driver");
            ModelState.Remove("Trips");
            ModelState.Remove("Expenses");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.VehicleId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DriverId"] = new SelectList(_context.Drivers, "DriverId", "FullName", vehicle.DriverId);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var vehicle = await _context.Vehicles
                .Include(v => v.Driver)
                .FirstOrDefaultAsync(m => m.VehicleId == id);

            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
                _context.Vehicles.Remove(vehicle);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}