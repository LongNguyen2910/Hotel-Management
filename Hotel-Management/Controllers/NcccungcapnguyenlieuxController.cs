using Hotel_Management.Helpers;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Management.Controllers
{
    public class NcccungcapnguyenlieuxController : Controller
    {
        private readonly AppDbContext _context;

        public NcccungcapnguyenlieuxController(AppDbContext context)
        {
            _context = context;
        }
        private bool IsAjaxRequest()
   => string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        // GET: Ncccungcapnguyenlieus
        [Authorize(Policy = "CanViewData")]
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            // Lấy lại từ session nếu searchString hoặc pageNumber null
            if (searchString == null && !pageNumber.HasValue)
            {
                var ss = HttpContext.Session.GetString("Ncccungcapnguyenlieux_Search");
                var sp = HttpContext.Session.GetInt32("Ncccungcapnguyenlieux_Page");
                if (!string.IsNullOrEmpty(ss))
                    searchString = ss;
                if (sp.HasValue)
                    pageNumber = sp;
            }

            var trimmed = (searchString ?? string.Empty).Trim();
            ViewData["CurrentFilter"] = searchString ?? string.Empty;

            var query = _context.Ncccungcapnguyenlieus.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                query = query.Where(n =>
                    EF.Functions.Like(n.Mancc ?? string.Empty, $"%{trimmed}%") ||
                    EF.Functions.Like(n.Manguyenlieu ?? string.Empty, $"%{trimmed}%") ||
                    EF.Functions.Like(n.Tennguyenlieu ?? string.Empty, $"%{trimmed}%"));
            }

            query = query.OrderBy(n => n.Mancc);

            int pageSize = 10;
            var model = await PaginatedList<Ncccungcapnguyenlieu>.CreateAsync(query, pageNumber ?? 1, pageSize);

            // Lưu lại session
            HttpContext.Session.SetString("Ncccungcapnguyenlieux_Search", searchString ?? string.Empty);
            HttpContext.Session.SetInt32("Ncccungcapnguyenlieux_Page", model.PageIndex);

            // Nếu là AJAX request thì render partial view
            if (IsAjaxRequest())
                return PartialView("_NcccungcapnguyenlieuxList", model); // tạo PartialView riêng

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
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
        public IActionResult Create()
        {
            ViewData["Mancc"] = new SelectList(_context.Nhacungcaps, "Manhacungcap", "Tennhacungcap");
            return View();
        }

        // POST: Create
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
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
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
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
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
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
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
        public async Task<IActionResult> Delete(string mancc, string manguyenlieu)
        {
            if (mancc == null || manguyenlieu == null) return NotFound();

            var entity = await _context.Ncccungcapnguyenlieus
                .FirstOrDefaultAsync(m => m.Mancc == mancc && m.Manguyenlieu == manguyenlieu);

            if (entity == null) return NotFound();

            return View(entity);
        }

        // POST: Delete
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
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
