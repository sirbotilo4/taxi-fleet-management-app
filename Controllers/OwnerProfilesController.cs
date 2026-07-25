using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiFleetManager.Data;
using TaxiFleetManager.Models;

namespace TaxiFleetManager.Controllers
{
    public class OwnerProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OwnerProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OwnerProfiles
        // Redirects straight to Edit since there's always exactly one record
        public async Task<IActionResult> Index()
        {
            var owner = await _context.OwnerProfiles.FirstOrDefaultAsync();
            if (owner == null)
                return NotFound();

            return RedirectToAction(nameof(Edit), new { id = owner.OwnerId });
        }

        // GET: OwnerProfiles/Edit/1
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ownerProfile = await _context.OwnerProfiles.FindAsync(id);
            if (ownerProfile == null)
                return NotFound();

            return View(ownerProfile);
        }

        // POST: OwnerProfiles/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OwnerId,BusinessName,FullName,ContactNumber,BankName,AccountNumber,BranchCode")] OwnerProfile ownerProfile)
        {
            if (id != ownerProfile.OwnerId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ownerProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerProfileExists(ownerProfile.OwnerId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Edit), new { id = ownerProfile.OwnerId });
            }
            return View(ownerProfile);
        }

        private bool OwnerProfileExists(int id)
        {
            return _context.OwnerProfiles.Any(e => e.OwnerId == id);
        }
    }
}