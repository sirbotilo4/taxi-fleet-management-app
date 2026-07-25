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
    public class TaxiRoutesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaxiRoutesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TaxiRoutes (Owner only — full list including inactive)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Routes.ToListAsync());
        }

        // GET: TaxiRoutes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var taxiRoute = await _context.Routes
                .Include(r => r.Trips)
                    .ThenInclude(t => t.Vehicle)
                .Include(r => r.Trips)
                    .ThenInclude(t => t.Driver)
                .FirstOrDefaultAsync(m => m.RouteId == id);

            if (taxiRoute == null)
                return NotFound();

            return View(taxiRoute);
        }

        // GET: TaxiRoutes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaxiRoutes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,StartPoint,EndPoint,StandardFare,RouteType,IsActive")] TaxiRoute taxiRoute)
        {
            // Trips is a navigation collection — not submitted by form
            ModelState.Remove("Trips");

            if (ModelState.IsValid)
            {
                _context.Add(taxiRoute);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taxiRoute);
        }

        // GET: TaxiRoutes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var taxiRoute = await _context.Routes.FindAsync(id);
            if (taxiRoute == null)
                return NotFound();

            return View(taxiRoute);
        }

        // POST: TaxiRoutes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RouteId,Name,StartPoint,EndPoint,StandardFare,RouteType,IsActive")] TaxiRoute taxiRoute)
        {
            if (id != taxiRoute.RouteId)
                return NotFound();

            ModelState.Remove("Trips");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taxiRoute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaxiRouteExists(taxiRoute.RouteId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(taxiRoute);
        }

        // GET: TaxiRoutes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var taxiRoute = await _context.Routes
                .FirstOrDefaultAsync(m => m.RouteId == id);

            if (taxiRoute == null)
                return NotFound();

            return View(taxiRoute);
        }

        // POST: TaxiRoutes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taxiRoute = await _context.Routes.FindAsync(id);
            if (taxiRoute != null)
                _context.Routes.Remove(taxiRoute);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaxiRouteExists(int id)
        {
            return _context.Routes.Any(e => e.RouteId == id);
        }
    }
}