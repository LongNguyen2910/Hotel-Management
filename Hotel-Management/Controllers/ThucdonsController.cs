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
    public class ThucdonsController : Controller
    {
        private readonly AppDbContext _context;

        public ThucdonsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Thucdons
        public async Task<IActionResult> Index()
        {
            var thucdons = _context.Thucdons;
                //.Include(t => t.Mamons)
                //.ThenInclude(m => m.Mamon);
            return View(await thucdons.ToListAsync());
        }

        // GET: Thucdons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thucdon = await _context.Thucdons
                .FirstOrDefaultAsync(m => m.Mathucdon == id);
            if (thucdon == null)
            {
                return NotFound();
            }

            return View(thucdon);
        }

        // GET: Thucdons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Thucdons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mathucdon,Ngayapdung,Ngaytao")] Thucdon thucdon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thucdon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thucdon);
        }

        // GET: Thucdons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thucdon = await _context.Thucdons.FindAsync(id);
            if (thucdon == null)
            {
                return NotFound();
            }
            return View(thucdon);
        }

        // POST: Thucdons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Mathucdon,Ngayapdung,Ngaytao")] Thucdon thucdon)
        {
            if (id != thucdon.Mathucdon)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thucdon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThucdonExists(thucdon.Mathucdon))
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
            return View(thucdon);
        }

        // GET: Thucdons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thucdon = await _context.Thucdons
                .FirstOrDefaultAsync(m => m.Mathucdon == id);
            if (thucdon == null)
            {
                return NotFound();
            }

            return View(thucdon);
        }

        // POST: Thucdons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thucdon = await _context.Thucdons.FindAsync(id);
            if (thucdon != null)
            {
                _context.Thucdons.Remove(thucdon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThucdonExists(int id)
        {
            return _context.Thucdons.Any(e => e.Mathucdon == id);
        }
    }
}
