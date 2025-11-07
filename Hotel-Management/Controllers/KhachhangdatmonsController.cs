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
    public class KhachhangdatmonsController : Controller
    {
        private readonly AppDbContext _context;

        public KhachhangdatmonsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Khachhangdatmons
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var datmons = _context.Khachhangdatmons
                .Include(d => d.MakhachhangNavigation)
                .Include(d => d.MamonNavigation)
                .AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                datmons = datmons.Where(d => d.Makhachhang.ToString().Contains(searchString));
            }
            var list = await datmons.ToListAsync();

            if (!list.Any())
            {
                ViewData["NoResults"] = true;
            }
            return View(list);
        }

        // GET: Khachhangdatmons/Details
        public async Task<IActionResult> Details(int makhachhang, string mamon, DateTime ngaydat)
        {
            var khachhangdatmon = await _context.Khachhangdatmons
                .Include(k => k.MakhachhangNavigation)
                .Include(k => k.MamonNavigation)
                .FirstOrDefaultAsync(m =>
                    m.Makhachhang == makhachhang &&
                    m.Mamon == mamon &&
                    m.Ngaydat == ngaydat);

            if (khachhangdatmon == null)
                return NotFound();

            return View(khachhangdatmon);
        }

        // GET: Khachhangdatmons/Create
        public IActionResult Create()
        {
            ViewData["Makhachhang"] = new SelectList(_context.Khachhangs, "Makhachhang", "Makhachhang");
            ViewData["Mamon"] = new SelectList(_context.Mons, "Mamon", "Tenmon");
            return View();
        }

        // POST: Khachhangdatmons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Makhachhang,Mamon,Ngaydat,Soluong")] Khachhangdatmon khachhangdatmon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khachhangdatmon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Makhachhang"] = new SelectList(_context.Khachhangs, "Makhachhang", "Makhachhang", khachhangdatmon.Makhachhang);
            ViewData["Mamon"] = new SelectList(_context.Mons, "Mamon", "Tenmon", khachhangdatmon.Mamon);
            return View(khachhangdatmon);
        }

        // GET: Khachhangdatmons/Edit
        public async Task<IActionResult> Edit(int makhachhang, string mamon, DateTime ngaydat)
        {
            var khachhangdatmon = await _context.Khachhangdatmons
                .FindAsync(makhachhang, mamon, ngaydat);

            if (khachhangdatmon == null)
                return NotFound();

            ViewData["Makhachhang"] = new SelectList(_context.Khachhangs, "Makhachhang", "Makhachhang", khachhangdatmon.Makhachhang);
            ViewData["Mamon"] = new SelectList(_context.Mons, "Mamon", "Tenmon", khachhangdatmon.Mamon);
            return View(khachhangdatmon);
        }

        // POST: Khachhangdatmons/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int makhachhang, string mamon, DateTime ngaydat, [Bind("Makhachhang,Mamon,Ngaydat,Soluong")] Khachhangdatmon khachhangdatmon)
        {
            if (makhachhang != khachhangdatmon.Makhachhang ||
                mamon != khachhangdatmon.Mamon ||
                ngaydat != khachhangdatmon.Ngaydat)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khachhangdatmon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachhangdatmonExists(makhachhang, mamon, ngaydat))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Makhachhang"] = new SelectList(_context.Khachhangs, "Makhachhang", "Makhachhang", khachhangdatmon.Makhachhang);
            ViewData["Mamon"] = new SelectList(_context.Mons, "Mamon", "Tenmon", khachhangdatmon.Mamon);
            return View(khachhangdatmon);
        }

        // GET: Khachhangdatmons/Delete
        public async Task<IActionResult> Delete(int makhachhang, string mamon, DateTime ngaydat)
        {
            var khachhangdatmon = await _context.Khachhangdatmons
                .Include(k => k.MakhachhangNavigation)
                .Include(k => k.MamonNavigation)
                .FirstOrDefaultAsync(m =>
                    m.Makhachhang == makhachhang &&
                    m.Mamon == mamon &&
                    m.Ngaydat == ngaydat);

            if (khachhangdatmon == null)
                return NotFound();

            return View(khachhangdatmon);
        }

        // POST: Khachhangdatmons/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int makhachhang, string mamon, DateTime ngaydat)
        {
            var khachhangdatmon = await _context.Khachhangdatmons
                .FindAsync(makhachhang, mamon, ngaydat);
            if (khachhangdatmon != null)
            {
                _context.Khachhangdatmons.Remove(khachhangdatmon);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool KhachhangdatmonExists(int makhachhang, string mamon, DateTime ngaydat)
        {
            return _context.Khachhangdatmons.Any(e =>
                e.Makhachhang == makhachhang &&
                e.Mamon == mamon &&
                e.Ngaydat == ngaydat);
        }
    }
}
