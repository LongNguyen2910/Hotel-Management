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
    public class LoaithietbisController : Controller
    {
        private readonly AppDbContext _context;

        public LoaithietbisController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Thietbis
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;

            var query = _context.Loaithietbis.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var trimmed = searchString.Trim();
                // use EF.Functions.Like for SQL LIKE; case-sensitivity depends on DB collation
                query = query.Where(t => EF.Functions.Like(t.Maloaithietbi, $"%{trimmed}%"));
            }

            var list = await query.ToListAsync();
            return View(list);
        }
        // GET: Loaithietbis/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaithietbi = await _context.Loaithietbis
                .FirstOrDefaultAsync(m => m.Maloaithietbi == id);
            if (loaithietbi == null)
            {
                return NotFound();
            }

            return View(loaithietbi);
        }

        // GET: Loaithietbis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Loaithietbis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Maloaithietbi,Tenloaithietbi")] Loaithietbi loaithietbi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaithietbi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaithietbi);
        }

        // GET: Loaithietbis/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaithietbi = await _context.Loaithietbis.FindAsync(id);
            if (loaithietbi == null)
            {
                return NotFound();
            }
            return View(loaithietbi);
        }

        // POST: Loaithietbis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Maloaithietbi,Tenloaithietbi")] Loaithietbi loaithietbi)
        {
            if (id != loaithietbi.Maloaithietbi)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaithietbi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaithietbiExists(loaithietbi.Maloaithietbi))
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
            return View(loaithietbi);
        }

        // GET: Loaithietbis/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaithietbi = await _context.Loaithietbis
                .FirstOrDefaultAsync(m => m.Maloaithietbi == id);
            if (loaithietbi == null)
            {
                return NotFound();
            }

            return View(loaithietbi);
        }

        // POST: Loaithietbis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var loaithietbi = await _context.Loaithietbis.FindAsync(id);
            if (loaithietbi != null)
            {
                _context.Loaithietbis.Remove(loaithietbi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaithietbiExists(string id)
        {
            return _context.Loaithietbis.Any(e => e.Maloaithietbi == id);
        }
    }
}
