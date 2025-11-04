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
    public class KhachhangsController : Controller
    {
        private readonly AppDbContext _context;

        public KhachhangsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Khachhangs
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var khachhangs = from k in _context.Khachhangs select k;
            if (!String.IsNullOrEmpty(searchString))
            {
                khachhangs = khachhangs
                    .Where(k => k.Hoten != null && k.Hoten.Contains(searchString));
            }
            var list = await khachhangs.ToListAsync();
            if (!list.Any())
            {
                ViewData["NoResults"] = true;
            }
            return View(list);
        }

        // GET: Khachhangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.Khachhangs
                .FirstOrDefaultAsync(m => m.Makhachhang == id);
            if (khachhang == null)
            {
                return NotFound();
            }

            return View(khachhang);
        }

        // GET: Khachhangs/Create
        public IActionResult Create(string? maphong)
        {
            var phong = _context.Phongs
                .Include(p => p.MaloaiphongNavigation)
                .FirstOrDefault(p => p.Maphong == maphong);
            ViewBag.Phong = phong;
            return View();
        }

        // POST: Khachhangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Hoten,Quoctich,Cccd,Sdt,Hochieu")] Khachhang khachhang, string? maphong)
        {
            if (!ModelState.IsValid)
                return View(khachhang);

            // Check CCCD trùng
            if (_context.Khachhangs.Count(k => k.Cccd == khachhang.Cccd) > 0)
            {
                ModelState.AddModelError("Cccd", "CCCD này đã tồn tại.");
                return View(khachhang);
            }

            // Sinh Makhachhang tự động
            int maxId = _context.Khachhangs.Count() > 0 ? _context.Khachhangs.Max(k => k.Makhachhang) : 0;
            khachhang.Makhachhang = maxId + 1;

            // Thêm khách hàng
            try
            {
                _context.Add(khachhang);
                await _context.SaveChangesAsync();
                if (maphong != null)
                {
                    Khachhangdatphong khdp = new Khachhangdatphong
                    {
                        Makhachhang = khachhang.Makhachhang,
                        Maphong = maphong,
                        Ngaydat = DateTime.Now
                    };
                    _context.Add(khdp);
                    var phong = await _context.Phongs.FindAsync(maphong);
                    if (phong != null)
                    {
                        phong.Tinhtrang = false;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Datphongs");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                string innerMessage = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"Có lỗi xảy ra khi lưu dữ liệu: {innerMessage}");
                return View(khachhang);
            }
        }




        // GET: Khachhangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.Khachhangs.FindAsync(id);
            if (khachhang == null)
            {
                return NotFound();
            }
            return View(khachhang);
        }

        // POST: Khachhangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Makhachhang,Hoten,Quoctich,Cccd,Sdt,Hochieu")] Khachhang khachhang)
        {
            if (id != khachhang.Makhachhang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khachhang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachhangExists(khachhang.Makhachhang))
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
            return View(khachhang);
        }

        // GET: Khachhangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.Khachhangs
                .FirstOrDefaultAsync(m => m.Makhachhang == id);
            if (khachhang == null)
            {
                return NotFound();
            }

            return View(khachhang);
        }

        // POST: Khachhangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khachhang = await _context.Khachhangs.FindAsync(id);
            if (khachhang != null)
            {
                _context.Khachhangs.Remove(khachhang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachhangExists(int id)
        {
            return _context.Khachhangs.Any(e => e.Makhachhang == id);
        }
    }
}
