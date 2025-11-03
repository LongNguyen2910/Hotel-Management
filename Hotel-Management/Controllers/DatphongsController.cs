using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Management.Controllers
{
    public class DatphongsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DatphongsController> _logger;

        public DatphongsController(AppDbContext context, ILogger<DatphongsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Datphongs
        // Search by mã phòng (Maphong)
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;
            var trimmed = (searchString ?? string.Empty).Trim();

            var query = _context.Phongs
                .Include(p => p.MaloaiphongNavigation)
                .Where(p => Convert.ToInt32(p.Tinhtrang) == 1)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                // use SQL LIKE 
                query = query.Where(p => EF.Functions.Like(p.MaloaiphongNavigation.Tenloaiphong, $"%{trimmed}%"));
            }

            var list = await query.ToListAsync();
            return View(list);
        }

        // GET: Datphongs/Create
        public IActionResult Create()
        {
            ViewBag.Maloaiphong = new SelectList(_context.Loaiphongs.OrderBy(l => l.Tenloaiphong)
                .Select(l => new { l.Maloaiphong, l.Tenloaiphong }), "Maloaiphong", "Tenloaiphong");

            // provide list of available devices 
            ViewBag.Thietbis = _context.Thietbis
                .OrderBy(t => t.Mathietbi)
                .Select(t => new { t.Mathietbi, t.Tenthietbi })
                .ToList();

            return View();
        }

        // POST: Datphongs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Maphong,Tenphong,Tinhtrang,Mota,Maloaiphong")] Phong phong, int[]? selectedThietbis)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Maloaiphong = new SelectList(_context.Loaiphongs.OrderBy(l => l.Tenloaiphong)
                    .Select(l => new { l.Maloaiphong, l.Tenloaiphong }), "Maloaiphong", "Tenloaiphong");

                ViewBag.Thietbis = _context.Thietbis
                    .OrderBy(t => t.Mathietbi)
                    .Select(t => new { t.Mathietbi, t.Tenthietbi })
                    .ToList();

                return View(phong);
            }

            try
            {
                // attach selected devices to the new room so EF will insert PHONGCHUATHIETBI rows
                if (selectedThietbis != null && selectedThietbis.Length > 0)
                {
                    foreach (var id in selectedThietbis.Distinct())
                    {
                        var tb = await _context.Thietbis.FindAsync(id);
                        if (tb != null)
                        {
                            
                            phong.Mathietbis.Add(tb);
                        }
                    }
                }

                _context.Phongs.Add(phong);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error when creating Phong (Maphong={Id})", phong?.Maphong);
                ModelState.AddModelError(string.Empty, "Unable to save changes to the database. See logs.");
                ViewBag.Maloaiphong = new SelectList(_context.Loaiphongs.OrderBy(l => l.Tenloaiphong)
                    .Select(l => new { l.Maloaiphong, l.Tenloaiphong }), "Maloaiphong", "Tenloaiphong");
                ViewBag.Thietbis = _context.Thietbis
                    .OrderBy(t => t.Mathietbi)
                    .Select(t => new { t.Mathietbi, t.Tenthietbi })
                    .ToList();
                return View(phong);
            }
        }

        // GET: Datphongs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var phong = await _context.Phongs
                .Include(p => p.MaloaiphongNavigation)
                .FirstOrDefaultAsync(m => m.Maphong == id);

            if (phong == null) return NotFound();
            return View(phong);
        }

        // POST: Datphongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong != null)
            {
                _context.Phongs.Remove(phong);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}