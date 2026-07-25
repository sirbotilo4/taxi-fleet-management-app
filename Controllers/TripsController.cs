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
    public class TripsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TripsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =============================================
        // OWNER SECTION — full fleet trip visibility
        // =============================================

        // GET: Trips (Owner — sees all trips)
        public async Task<IActionResult> Index()
        {
            var trips = _context.Trips
                .Include(t => t.Driver)
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .OrderByDescending(t => t.TripDate);
            return View(await trips.ToListAsync());
        }

        // GET: Trips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var trip = await _context.Trips
                .Include(t => t.Driver)
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(m => m.TripId == id);

            if (trip == null)
                return NotFound();

            return View(trip);
        }

        // GET: Trips/Edit/5 (Owner can edit any trip)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
                return NotFound();

            PopulateDropdowns(trip.DriverId, trip.RouteId, trip.VehicleId);
            return View(trip);
        }

        // POST: Trips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TripId,VehicleId,DriverId,RouteId,TripDate,PassengerCount,AmountCollected,PaymentMethod,PaymentReference")] Trip trip)
        {
            if (id != trip.TripId)
                return NotFound();

            ModelState.Remove("Driver");
            ModelState.Remove("Route");
            ModelState.Remove("Vehicle");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripExists(trip.TripId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(trip.DriverId, trip.RouteId, trip.VehicleId);
            return View(trip);
        }

        // GET: Trips/Delete/5 (Owner only)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var trip = await _context.Trips
                .Include(t => t.Driver)
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(m => m.TripId == id);

            if (trip == null)
                return NotFound();

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip != null)
                _context.Trips.Remove(trip);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =============================================
        // DRIVER SECTION — PIN-gated, scoped to self
        // =============================================

        // GET: Trips/DriverLogin — driver selects their name + enters PIN
        public IActionResult DriverLogin()
        {
            ViewData["DriverId"] = new SelectList(_context.Drivers.Where(d => d.Status == "Active"), "DriverId", "FullName");
            return View();
        }

        // POST: Trips/DriverLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DriverLogin(int driverId, string pin)
        {
            var driver = await _context.Drivers.FindAsync(driverId);

            if (driver == null || driver.Pin != pin)
            {
                ModelState.AddModelError("", "Invalid driver selection or PIN.");
                ViewData["DriverId"] = new SelectList(_context.Drivers.Where(d => d.Status == "Active"), "DriverId", "FullName");
                return View();
            }

            // Store driverId in session so we know who's logged in
            HttpContext.Session.SetInt32("DriverId", driverId);
            return RedirectToAction(nameof(DriverDashboard));
        }

        // GET: Trips/DriverDashboard — driver sees own trips + stats
        public async Task<IActionResult> DriverDashboard()
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return RedirectToAction(nameof(DriverLogin));

            var trips = await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Vehicle)
                .Where(t => t.DriverId == driverId)
                .OrderByDescending(t => t.TripDate)
                .ToListAsync();

            ViewBag.DriverName = (await _context.Drivers.FindAsync(driverId))?.FullName;
            ViewBag.TotalTrips = trips.Count;
            ViewBag.TotalRevenue = trips.Sum(t => t.AmountCollected);

            return View(trips);
        }

        // GET: Trips/LogTrip — driver logs a new trip
        public IActionResult LogTrip()
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return RedirectToAction(nameof(DriverLogin));

            // Only active routes in dropdown
            ViewData["RouteId"] = new SelectList(
                _context.Routes.Where(r => r.IsActive),
                "RouteId", "Name");

            // Only the vehicle currently assigned to this driver
            ViewData["VehicleId"] = new SelectList(
                _context.Vehicles.Where(v => v.DriverId == driverId),
                "VehicleId", "RegistrationNumber");

            return View();
        }

        // POST: Trips/LogTrip
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogTrip([Bind("VehicleId,RouteId,PassengerCount,AmountCollected,PaymentMethod,PaymentReference")] Trip trip)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return RedirectToAction(nameof(DriverLogin));

            ModelState.Remove("Driver");
            ModelState.Remove("Route");
            ModelState.Remove("Vehicle");
            ModelState.Remove("TripDate");

            if (ModelState.IsValid)
            {
                trip.DriverId = driverId.Value;
                trip.TripDate = DateTime.Now;
                _context.Add(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DriverDashboard));
            }

            ViewData["RouteId"] = new SelectList(
                _context.Routes.Where(r => r.IsActive),
                "RouteId", "Name", trip.RouteId);

            ViewData["VehicleId"] = new SelectList(
                _context.Vehicles.Where(v => v.DriverId == driverId),
                "VehicleId", "RegistrationNumber", trip.VehicleId);

            return View(trip);
        }

        // GET: Trips/DriverEdit/5 — driver edits own trip only
        public async Task<IActionResult> DriverEdit(int? id)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return RedirectToAction(nameof(DriverLogin));

            if (id == null)
                return NotFound();

            var trip = await _context.Trips.FindAsync(id);
            if (trip == null || trip.DriverId != driverId)
                return Forbid(); // Can't edit another driver's trip

            ViewData["RouteId"] = new SelectList(
                _context.Routes.Where(r => r.IsActive),
                "RouteId", "Name", trip.RouteId);

            ViewData["VehicleId"] = new SelectList(
                _context.Vehicles.Where(v => v.DriverId == driverId),
                "VehicleId", "RegistrationNumber", trip.VehicleId);

            return View(trip);
        }

        // POST: Trips/DriverEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DriverEdit(int id, [Bind("TripId,VehicleId,RouteId,TripDate,PassengerCount,AmountCollected,PaymentMethod,PaymentReference")] Trip trip)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return RedirectToAction(nameof(DriverLogin));

            if (id != trip.TripId)
                return NotFound();

            // Verify this trip belongs to the logged-in driver
            var existingTrip = await _context.Trips.AsNoTracking().FirstOrDefaultAsync(t => t.TripId == id);
            if (existingTrip == null || existingTrip.DriverId != driverId)
                return Forbid();

            ModelState.Remove("Driver");
            ModelState.Remove("Route");
            ModelState.Remove("Vehicle");

            if (ModelState.IsValid)
            {
                trip.DriverId = driverId.Value; // Lock DriverId — driver can't reassign a trip
                try
                {
                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripExists(trip.TripId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(DriverDashboard));
            }

            ViewData["RouteId"] = new SelectList(
                _context.Routes.Where(r => r.IsActive),
                "RouteId", "Name", trip.RouteId);

            ViewData["VehicleId"] = new SelectList(
                _context.Vehicles.Where(v => v.DriverId == driverId),
                "VehicleId", "RegistrationNumber", trip.VehicleId);

            return View(trip);
        }

        // GET: Trips/DriverLogout
        public IActionResult DriverLogout()
        {
            HttpContext.Session.Remove("DriverId");
            return RedirectToAction(nameof(DriverLogin));
        }

        // =============================================
        // HELPERS
        // =============================================

        private void PopulateDropdowns(int? driverId = null, int? routeId = null, int? vehicleId = null)
        {
            ViewData["DriverId"] = new SelectList(_context.Drivers, "DriverId", "FullName", driverId);
            ViewData["RouteId"] = new SelectList(_context.Routes, "RouteId", "Name", routeId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "RegistrationNumber", vehicleId);
        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.TripId == id);
        }
    }
}