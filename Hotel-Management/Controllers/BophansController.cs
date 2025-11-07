using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Management.Models;
using Hotel_Management.Helpers;

namespace Hotel_Management.Controllers
{
    public class BophansController : Controller
    {
        private readonly AppDbContext _context;

        public BophansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bophans
        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 10;
            return View(await PaginatedList<Bophan>.CreateAsync(_context.Bophans.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Bophans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bophans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create([Bind("Mabophan,Tenbophan,Ngaythanhlap")] Bophan bophan)
        {
            if (BophanExists(bophan.Mabophan))
            {
                ModelState.AddModelError("Mabophan", "Tên bộ phận đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(bophan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bophan);
        }

        // GET: Bophans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bophan = await _context.Bophans.FindAsync(id);
            if (bophan == null)
            {
                return NotFound();
            }
            return View(bophan);
        }

        // POST: Bophans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Mabophan,Tenbophan,Ngaythanhlap")] Bophan bophan)
        {
            if (id != bophan.Mabophan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bophan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BophanExists(bophan.Mabophan))
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
            return View(bophan);
        }

        // GET: Bophans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bophan = await _context.Bophans
                .FirstOrDefaultAsync(m => m.Mabophan == id);
            if (bophan == null)
            {
                return NotFound();
            }

            return View(bophan);
        }

        // POST: Bophans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bophan = await _context.Bophans
                .Include(bp => bp.Nhanviens)
                .FirstOrDefaultAsync(m => m.Mabophan == id);
            if (bophan != null)
            {
                bophan.Nhanviens.Clear();
                _context.Bophans.Remove(bophan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BophanExists(string id)
        {
            return _context.Bophans.Where(e => e.Mabophan == id).Count() > 0;
        }
    }
}
