using System;
using System.Linq;
using System.Threading.Tasks;
using Hotel_Management.Helpers;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Controllers
{
    public class NcccungcapnguyenlieuxController : Controller
    {
        private readonly AppDbContext _context;

        public NcccungcapnguyenlieuxController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ncccungcapnguyenlieux
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;

            var query = _context.Ncccungcapnguyenlieus.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var trimmed = searchString.Trim();
                query = query.Where(n =>
                    EF.Functions.Like(n.Mancc ?? string.Empty, $"%{trimmed}%") ||
                    EF.Functions.Like(n.Manguyenlieu ?? string.Empty, $"%{trimmed}%") ||
                    EF.Functions.Like(n.Tennguyenlieu ?? string.Empty, $"%{trimmed}%"));
            }

            query = query.OrderBy(n => n.Mancc);

            int pageSize = 10;
            var model = await PaginatedList<Ncccungcapnguyenlieu>.CreateAsync(query, pageNumber ?? 1, pageSize);
            return View(model);
        }

        // GET: Details
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var item = await _context.Ncccungcapnguyenlieus
                .Include(n => n.ManccNavigation)
                .FirstOrDefaultAsync(m => m.Mancc == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }


        // GET: Create
        public IActionResult Create()
        {
            ViewData["Mancc"] = new SelectList(_context.Nhacungcaps, "Manhacungcap", "Tennhacungcap");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mancc,Manguyenlieu,Tennguyenlieu,Luong,Tiennguyenlieu,Ngaysanxuat,Ngaynhap,Hansudung")] Ncccungcapnguyenlieu entity)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mancc"] = new SelectList(_context.Nhacungcaps, "Manhacungcap", "Tennhacungcap", entity.Mancc);
                return View(entity);
            }

            _context.Add(entity);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm nhà cung cấp nguyên liệu thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public async Task<IActionResult> Edit(string mancc)
        {
            if (string.IsNullOrEmpty(mancc))
                return NotFound();

            var entity = await _context.Ncccungcapnguyenlieus
                .FirstOrDefaultAsync(n => n.Mancc == mancc);

            if (entity == null)
                return NotFound();

            return View(entity);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Mancc,Manguyenlieu,Tennguyenlieu,Ngaynhap,Ngaysanxuat,Hansudung,Luong,Tiennguyenlieu")] Ncccungcapnguyenlieu model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Tìm entity theo khóa kép
            var entity = await _context.Ncccungcapnguyenlieus
                .FirstOrDefaultAsync(n => n.Mancc == model.Mancc && n.Manguyenlieu == model.Manguyenlieu);

            if (entity == null)
                return NotFound();

            // Cập nhật các field thông tin
            entity.Tennguyenlieu = model.Tennguyenlieu;
            entity.Ngaynhap = model.Ngaynhap;
            entity.Ngaysanxuat = model.Ngaysanxuat;
            entity.Hansudung = model.Hansudung;
            entity.Luong = model.Luong;
            entity.Tiennguyenlieu = model.Tiennguyenlieu;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction(nameof(Index));
        }



        // GET: Delete
        public async Task<IActionResult> Delete(string mancc, string manguyenlieu)
        {
            if (mancc == null || manguyenlieu == null) return NotFound();

            var entity = await _context.Ncccungcapnguyenlieus
                .FirstOrDefaultAsync(m => m.Mancc == mancc && m.Manguyenlieu == manguyenlieu);

            if (entity == null) return NotFound();

            return View(entity);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string mancc, string manguyenlieu)
        {
            var entity = await _context.Ncccungcapnguyenlieus.FindAsync(mancc, manguyenlieu);
            if (entity != null)
            {
                _context.Ncccungcapnguyenlieus.Remove(entity);
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "Đã xóa thành công.";
            return RedirectToAction(nameof(Index));
        }

        private bool Exists(string mancc, string manguyenlieu)
        {
            return _context.Ncccungcapnguyenlieus.Any(e => e.Mancc == mancc && e.Manguyenlieu == manguyenlieu);
        }
    }
}
