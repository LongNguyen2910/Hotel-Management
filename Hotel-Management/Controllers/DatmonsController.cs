using Hotel_Management.Helpers;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Management.Controllers
{
    public class DatmonsController : Controller
    {
        private readonly AppDbContext _context;

        public DatmonsController(AppDbContext context)
        {
            _context = context;
        }
        // Add Ajax
        private bool IsAjaxRequest()
    => string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        // GET: Datmons
        [Authorize(Policy = "CanViewData")]
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            // Nếu không có searchString hoặc pageNumber, lấy từ session
            if (searchString == null && !pageNumber.HasValue)
            {
                var ss = HttpContext.Session.GetString("Datmons_Search");
                var sp = HttpContext.Session.GetInt32("Datmons_Page");
                if (!string.IsNullOrEmpty(ss))
                    searchString = ss;
                if (sp.HasValue)
                    pageNumber = sp;
            }

            var trimmed = (searchString ?? string.Empty).Trim();
            int pageSize = 10;
            ViewData["CurrentFilter"] = searchString ?? string.Empty;

            var query = _context.Mons.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                // Sử dụng SQL LIKE để tìm theo tên món
                query = query.Where(m => EF.Functions.Like(m.Tenmon ?? "", $"%{trimmed}%"));
            }

            query = query.OrderBy(m => m.Mamon);

            var model = await PaginatedList<Mon>.CreateAsync(query, pageNumber ?? 1, pageSize);

            // Lưu session để nhớ tìm kiếm và trang hiện tại
            HttpContext.Session.SetString("Datmons_Search", searchString ?? string.Empty);
            HttpContext.Session.SetInt32("Datmons_Page", model.PageIndex);

            // Nếu là AJAX request, trả về partial view
            if (IsAjaxRequest())
                return PartialView("_DatmonsList", model);

            return View(model);
        }

        // GET: Datmons/Create?mamon=xxx
        [Authorize(Roles = "Admin, Lễ tân, Quản lý khách sạn, Quản lý nhà hàng, Nhân viên")]
        public async Task<IActionResult> Create(string mamon)
        {
            if (string.IsNullOrEmpty(mamon))
            {
                TempData["Error"] = "Không tìm thấy món cần đặt.";
                return RedirectToAction("Index");
            }

            var mon = await _context.Mons
                .FirstOrDefaultAsync(m => m.Mamon == mamon);

            if (mon == null)
            {
                TempData["Error"] = "Món không tồn tại.";
                return RedirectToAction("Index");
            }

            return View(new List<Mon> { mon });
        }

        // POST: Datmons/SaveOrder
        [Authorize(Roles = "Admin, Lễ tân, Quản lý khách sạn, Quản lý nhà hàng, Nhân viên")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveOrder(int makhachhang, string[] mamon, int[] soluong)
        {
            if (mamon == null || soluong == null || mamon.Length == 0)
            {
                TempData["Error"] = "Không có món nào được chọn.";
                return RedirectToAction("Index");
            }

            var khachhang = await _context.Khachhangs.FindAsync(makhachhang);
            if (khachhang == null)
            {
                TempData["Error"] = $"Không tìm thấy khách hàng có mã {makhachhang}.";
                return RedirectToAction("Index");
            }

            for (int i = 0; i < mamon.Length; i++)
            {
                string maMon = mamon[i];
                int sl = soluong[i];

                var existing = await _context.Khachhangdatmons
                    .FirstOrDefaultAsync(k => k.Makhachhang == makhachhang && k.Mamon == maMon);

                if (existing != null)
                {
                    existing.Soluong = (existing.Soluong ?? 0) + sl;
                }
                else
                {
                    _context.Khachhangdatmons.Add(new Khachhangdatmon
                    {
                        Makhachhang = makhachhang,
                        Mamon = maMon,
                        Soluong = sl,
                        Ngaydat = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();
            await CapNhatHoaDonAsync(makhachhang);

            TempData["Success"] = "Đặt món và cập nhật hoá đơn thành công!";
            return RedirectToAction("Index", "Khachhangdatmons");
        }

        // Cập nhật hóa đơn
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
