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

        private async Task CapNhatHoaDonAsync(int makhachhang, string maphong)
        {
            // Tìm hoặc tạo hóa đơn cho khách hàng
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
                    Giamon = 0,
                    Manv = "NV001"
                };

                _context.Hoadons.Add(hoadon);
                await _context.SaveChangesAsync(); // Lưu hóa đơn mới
            }

            // Load phòng và loại phòng (để có giá)
            var phong = await _context.Phongs
                .Include(p => p.MaloaiphongNavigation)
                .FirstOrDefaultAsync(p => p.Maphong == maphong);

            if (phong == null || phong.MaloaiphongNavigation == null)
                return;

            decimal giaPhong = phong.MaloaiphongNavigation.Gia ?? 0m;

            // Kiểm tra khách có dòng đặt phòng này chưa
            var datphong = await _context.Khachhangdatphongs
                .FirstOrDefaultAsync(dp => dp.Makhachhang == makhachhang && dp.Maphong == maphong);

            if (datphong == null)
            {
                datphong = new Khachhangdatphong
                {
                    Makhachhang = makhachhang,
                    Maphong = maphong,
                    Ngaydat = DateTime.Now,
                    Ngaycheckin = null,
                    Ngaycheckout = null
                };

                _context.Khachhangdatphongs.Add(datphong);
                await _context.SaveChangesAsync(); // Lưu bản ghi đặt phòng
            }

            // Cộng giá phòng vào hóa đơn
            hoadon.Giaphong += giaPhong;

            // Đánh dấu entity đã thay đổi và lưu lại
            _context.Hoadons.Update(hoadon);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Cập nhật hóa đơn cho KH {makhachhang}: +{giaPhong} (Tổng {hoadon.Giaphong})");
        }


        // Xử lý đặt phòng
        [HttpPost]
        public async Task<IActionResult> DatPhong(int makhachhang, string maphong)
        {
            var khachhang = await _context.Khachhangs.FindAsync(makhachhang);
            if (khachhang == null)
            {
                TempData["Error"] = $"Không tìm thấy khách hàng có mã {makhachhang}.";
                return RedirectToAction("Index");
            }

            var phong = await _context.Phongs
                .Include(p => p.MaloaiphongNavigation)
                .FirstOrDefaultAsync(p => p.Maphong == maphong);

            if (phong == null)
            {
                TempData["Error"] = $"Không tìm thấy phòng có mã {maphong}.";
                return RedirectToAction("Index");
            }

            if (!phong.Tinhtrang)
            {
                TempData["Error"] = $"Phòng {maphong} đã được đặt rồi.";
                return RedirectToAction(nameof(Index));
            }

            // Cập nhật hóa đơn và danh sách đặt phòng
            await CapNhatHoaDonAsync(makhachhang, maphong);

            // Cập nhật trạng thái phòng: đã được đặt
            phong.Tinhtrang = false;
            _context.Update(phong);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đặt phòng {maphong} cho khách hàng {makhachhang} thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Datphongs
        // Search by mã phòng (Maphong)
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;
            var trimmed = (searchString ?? string.Empty).Trim().ToUpper();

            var query = _context.Phongs
                .Include(p => p.MaloaiphongNavigation)
                .Where(p => Convert.ToInt32(p.Tinhtrang) == 1)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                // use SQL LIKE 
                query = query.Where(p => EF.Functions.Like(p.MaloaiphongNavigation.Tenloaiphong.ToUpper(), $"%{trimmed}%"));
            }

            var list = await query.ToListAsync();
            return View(list);
        }

        // GET: Datphongs/Create
        [HttpGet]
        public async Task<IActionResult> Create(string maphong)
        {
            if (string.IsNullOrEmpty(maphong))
                return RedirectToAction(nameof(Index));

            var phong = await _context.Phongs
                .Include(p => p.MaloaiphongNavigation)
                .FirstOrDefaultAsync(p => p.Maphong == maphong);

            if (phong == null)
            {
                TempData["Error"] = "Không tìm thấy phòng.";
                return RedirectToAction(nameof(Index));
            }

            return View(phong);
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

                return View();
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