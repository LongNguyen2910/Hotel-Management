using Hotel_Management.Helpers;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Management.Controllers
{
    public class KhachhangdatmonsController : Controller
    {
        private readonly AppDbContext _context;

        public KhachhangdatmonsController(AppDbContext context)
        {
            _context = context;
        }
        private bool IsAjaxRequest()
=> string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        // GET: Khachhangdatmons
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;

            var datmons = _context.Khachhangdatmons
                .Include(d => d.MakhachhangNavigation)
                .Include(d => d.MamonNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                datmons = datmons.Where(d => d.Makhachhang.ToString().Contains(searchString));
            }

            datmons = datmons.OrderBy(d => d.Makhachhang).ThenBy(d => d.Mamon);

            int pageSize = 10;
            var model = await PaginatedList<Khachhangdatmon>.CreateAsync(datmons.AsNoTracking(), pageNumber ?? 1, pageSize);

            if (!model.Any())
                ViewData["NoResults"] = true;

            // Nếu là AJAX thì chỉ render phần danh sách
            if (IsAjaxRequest())
                return PartialView("_KhachhangdatmonsList", model);

            // Còn nếu là request bình thường thì render cả view
            return View(model);
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

            try
            {
                var existing = await _context.Khachhangdatmons
                       .FirstOrDefaultAsync(k => k.Makhachhang == makhachhang && k.Mamon == mamon && k.Ngaydat == ngaydat);

                if (existing == null)
                    return NotFound();

                existing.Soluong = khachhangdatmon.Soluong;
                await _context.SaveChangesAsync();
                await CapNhatHoaDonAsync(makhachhang);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KhachhangdatmonExists(makhachhang, mamon, ngaydat))
                    return NotFound();
                else
                    throw;
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
        private async Task CapNhatHoaDonAsync(int makhachhang)
        {
            var hoadon = await _context.Hoadons
                .FirstOrDefaultAsync(h => h.Makhachhang == makhachhang);

            if (hoadon == null)
            {
                var lastMa = await _context.Hoadons
                    .OrderByDescending(h => h.Mahoadon)
                    .Select(h => h.Mahoadon)
                    .FirstOrDefaultAsync();

                string newMa = "HD001";
                if (!string.IsNullOrEmpty(lastMa) && lastMa.Length > 2 &&
                    int.TryParse(lastMa.Substring(2), out int num))
                {
                    newMa = "HD" + (num + 1).ToString("D3");
                }

                hoadon = new Hoadon
                {
                    Mahoadon = newMa,
                    Makhachhang = makhachhang,
                    Ngaylap = DateTime.Now,
                    Giaphong = 0,
                    Giamon = 0
                };

                _context.Hoadons.Add(hoadon);
                await _context.SaveChangesAsync();
            }

            var tongTienMon = await _context.Khachhangdatmons
                .Include(dm => dm.MamonNavigation)
                .Where(dm => dm.Makhachhang == makhachhang)
                .SumAsync(dm => (dm.MamonNavigation!.Gia ?? 0) * (dm.Soluong ?? 1));

            hoadon.Giamon = tongTienMon;
            hoadon.Ngaylap = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
}
