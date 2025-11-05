using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Management.Models;

namespace Hotel_Management.Controllers
{
    public class MonsController : Controller
    {
        private readonly AppDbContext _context;

        public MonsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Mons
        public async Task<IActionResult> Index()
        {
            return View(await _context.Mons.ToListAsync());
        }

        // GET: Mons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mamon,Tenmon,Gia")] Mon mon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mon);
        }

        // GET: Mons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mon = await _context.Mons.FindAsync(id);
            if (mon == null)
            {
                return NotFound();
            }
            return View(mon);
        }

        // POST: Mons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Mamon,Tenmon,Gia")] Mon mon)
        {
            if (id != mon.Mamon)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonExists(mon.Mamon))
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
            return View(mon);
        }

        // GET: Mons/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mon = await _context.Mons
                .FirstOrDefaultAsync(m => m.Mamon == id);
            if (mon == null)
            {
                return NotFound();
            }

            return View(mon);
        }

        // POST: Mons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var mon = await _context.Mons.FindAsync(id);
            if (mon != null)
            {
                _context.Mons.Remove(mon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonExists(string id)
        {
            return _context.Mons.Any(e => e.Mamon == id);
        }
    }
}
