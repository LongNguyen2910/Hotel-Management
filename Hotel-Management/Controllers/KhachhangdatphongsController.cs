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
    public class KhachhangdatphongsController : Controller
    {
        private readonly AppDbContext _context;

        public KhachhangdatphongsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Khachhangdatphongs
        public async Task<IActionResult> Index(string searchString)
        {

            ViewData["CurrentFilter"] = searchString ?? string.Empty;
            var trimmed = (searchString ?? string.Empty).Trim().ToUpper();

            var query = _context.Khachhangdatphongs
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                // use SQL LIKE 
                query = query.Where(p => p.Makhachhang.ToString() == searchString);
            }

            var list = await query.ToListAsync();
            if (!list.Any())
            {
                ViewData["NoResults"] = true;
            }
            return View(list);
        }

        // GET: Khachhangdatphongs/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhangdatphong = await _context.Khachhangdatphongs
                .Include(k => k.MakhachhangNavigation)
                .Include(k => k.MaphongNavigation)
                .FirstOrDefaultAsync(m => m.Maphong == id);
            if (khachhangdatphong == null)
            {
                return NotFound();
            }

            return View(khachhangdatphong);
        }

        // GET: Khachhangdatphongs/Create
        public IActionResult Create()
        {
            ViewData["Makhachhang"] = new SelectList(_context.Khachhangs, "Makhachhang", "Makhachhang");
            ViewData["Maphong"] = new SelectList(_context.Phongs, "Maphong", "Maphong");
            return View();
        }

        // POST: Khachhangdatphongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Makhachhang,Maphong,Ngaydat,Ngaycheckin,Ngaycheckout")] Khachhangdatphong khachhangdatphong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khachhangdatphong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Makhachhang"] = new SelectList(_context.Khachhangs, "Makhachhang", "Makhachhang", khachhangdatphong.Makhachhang);
            ViewData["Maphong"] = new SelectList(_context.Phongs, "Maphong", "Maphong", khachhangdatphong.Maphong);
            return View(khachhangdatphong);
        }

        // GET: Khachhangdatphongs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhangdatphong = await _context.Khachhangdatphongs.FindAsync(id);
            if (khachhangdatphong == null)
            {
                return NotFound();
            }
            ViewData["Makhachhang"] = new SelectList(_context.Khachhangs, "Makhachhang", "Makhachhang", khachhangdatphong.Makhachhang);
            ViewData["Maphong"] = new SelectList(_context.Phongs, "Maphong", "Maphong", khachhangdatphong.Maphong);
            return View(khachhangdatphong);
        }

        // POST: Khachhangdatphongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Makhachhang,Maphong,Ngaydat,Ngaycheckin,Ngaycheckout")] Khachhangdatphong khachhangdatphong)
        {
            if (id != khachhangdatphong.Makhachhang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khachhangdatphong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachhangdatphongExists(khachhangdatphong.Maphong))
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
            ViewData["Makhachhang"] = new SelectList(_context.Khachhangs, "Makhachhang", "Makhachhang", khachhangdatphong.Makhachhang);
            ViewData["Maphong"] = new SelectList(_context.Phongs, "Maphong", "Maphong", khachhangdatphong.Maphong);
            return View(khachhangdatphong);
        }

        // GET: Khachhangdatphongs/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhangdatphong = await _context.Khachhangdatphongs
                .Include(k => k.MakhachhangNavigation)
                .Include(k => k.MaphongNavigation)
                .FirstOrDefaultAsync(m => m.Maphong == id);
            if (khachhangdatphong == null)
            {
                return NotFound();
            }

            return View(khachhangdatphong);
        }

        // POST: Khachhangdatphongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khachhangdatphong = await _context.Khachhangdatphongs
                .Include(k => k.MakhachhangNavigation)
                .Include(k => k.MaphongNavigation)
                .FirstOrDefaultAsync(m => m.Maphong == id);
            if (khachhangdatphong != null)
            {
                khachhangdatphong.MaphongNavigation.Tinhtrang = true;
                _context.Khachhangdatphongs.Remove(khachhangdatphong);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachhangdatphongExists(string id)
        {
            return _context.Khachhangdatphongs.Any(e => e.Maphong == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int makhachhang, string maphong)
        {
            if (string.IsNullOrWhiteSpace(maphong))
                return BadRequest();

            var booking = await _context.Khachhangdatphongs
                .FirstOrDefaultAsync(x => x.Makhachhang == makhachhang && x.Maphong == maphong);

            if (booking == null)
                return NotFound();

            booking.Ngaycheckin = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(int makhachhang, string maphong)
        {
            if (string.IsNullOrWhiteSpace(maphong))
                return BadRequest();

            var booking = await _context.Khachhangdatphongs
                .FirstOrDefaultAsync(x => x.Makhachhang == makhachhang && x.Maphong == maphong);

            if (booking == null)
                return NotFound();

            booking.Ngaycheckout = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
