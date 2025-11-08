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
    [Route("~/[controller]/[action]")]
    public class HoadonsController : Controller
    {
        private readonly AppDbContext _context;

        public HoadonsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CapNhatHoaDon(int makhachhang)
        {
            // Lấy thông tin khách hàng cùng dữ liệu liên quan (phòng + món)
            var khach = await _context.Khachhangs
                .Include(k => k.Khachhangdatphongs)
                    .ThenInclude(dp => dp.MaphongNavigation)
                        .ThenInclude(p => p.MaloaiphongNavigation)
                .Include(k => k.Khachhangdatmons)
                    .ThenInclude(dm => dm.MamonNavigation)
                .FirstOrDefaultAsync(k => k.Makhachhang == makhachhang);

            // Nếu không tìm thấy khách hàng thì quay lại danh sách
            if (khach == null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng!";
                return RedirectToAction("Index", "Khachhangs");
            }

            // Kiểm tra xem khách đã có hóa đơn chưa
            var hoadon = await _context.Hoadons
                .FirstOrDefaultAsync(h => h.Makhachhang == makhachhang);

            // Nếu chưa có thì tự động tạo hóa đơn mới
            if (hoadon == null)
            {
                // Lấy mã hóa đơn cuối cùng trong CSDL
                var lastMa = await _context.Hoadons
                    .OrderByDescending(h => h.Mahoadon)
                    .Select(h => h.Mahoadon)
                    .FirstOrDefaultAsync();

                // Tạo mã hóa đơn mới
                var newMa = "HĐ001";
                if (!string.IsNullOrEmpty(lastMa) && lastMa.Length > 2 &&
                    int.TryParse(lastMa.Substring(2), out int number))
                {
                    newMa = "HĐ" + (number + 1).ToString("D3");
                }

                // Tạo đối tượng hóa đơn mới
                hoadon = new Hoadon
                {
                    Mahoadon = newMa,
                    Ngaylap = DateTime.Now,
                    Makhachhang = makhachhang,
                    // Lấy mã NV — sau này thay bằng nhân viên đăng nhập thực tế
                    Manv = User.Identity?.Name ?? "NV001",
                    Giaphong = 0,
                    Giamon = 0
                };

                // Thêm hóa đơn mới vào context
                _context.Hoadons.Add(hoadon);
                await _context.SaveChangesAsync(); // Lưu tạm để có sẵn mã HĐ
            }

            //Tính tổng tiền phòng khách đã đặt
            decimal tongTienPhong = 0;
            if (khach.Khachhangdatphongs?.Any() == true)
            {
                foreach (var dp in khach.Khachhangdatphongs)
                {
                    // Lấy giá phòng từ loại phòng
                    var giaPhong = dp.MaphongNavigation?.MaloaiphongNavigation?.Gia ?? 0;

                    // Tính số ngày ở (ngày checkout - ngày checkin)
                    int soNgay = (dp.Ngaycheckout?.Date - dp.Ngaycheckin?.Date)?.Days ?? 1;
                    if (soNgay <= 0) soNgay = 1; // nếu trùng ngày thì tính 1 đêm

                    // Cộng dồn tổng tiền phòng
                    tongTienPhong += giaPhong * soNgay;
                }
            }

            // Tính tổng tiền món khách đã đặt
            decimal tongTienMon = 0;
            if (khach.Khachhangdatmons?.Any() == true)
            {
                tongTienMon = khach.Khachhangdatmons.Sum(dm =>
                {
                    var giaMon = dm.MamonNavigation?.Gia ?? 0;
                    var soLuong = dm.Soluong ?? 1;
                    return giaMon * soLuong;
                });
            }

            // Cập nhật lại thông tin hóa đơn
            hoadon.Giaphong = tongTienPhong;
            hoadon.Giamon = tongTienMon;
            hoadon.Ngaylap = DateTime.Now;

            // Lưu thay đổi vào CSDL
            await _context.SaveChangesAsync();

            // Thông báo và chuyển hướng
            TempData["Success"] = $"Cập nhật hóa đơn {hoadon.Mahoadon} thành công!";
            return RedirectToAction("Details", new { id = hoadon.Mahoadon });
        }


        // GET: Hoadons
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var hoadons = from h in _context.Hoadons
                          select h;
            // Nếu người dùng nhập tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                hoadons = hoadons.Where(h =>
                    h.Mahoadon.Contains(searchString) ||
                    h.Makhachhang.ToString().Contains(searchString)
                );
            }
            var list = await hoadons.ToListAsync();
            if (!list.Any())
            {
                ViewData["NoResults"] = true;
            }
            return View(list);
        }

        // GET: Hoadons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var hoadon = await _context.Hoadons
                .Include(h => h.MakhachhangNavigation)
                .Include(h => h.ManvNavigation)
                .FirstOrDefaultAsync(m => m.Mahoadon == id);

            if (hoadon == null)
                return NotFound();

            return View(hoadon);
        }

        // Cho phép xóa hóa đơn (nếu cần)
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            var hoadon = await _context.Hoadons.FindAsync(id);
            if (hoadon == null)
                return NotFound();

            _context.Hoadons.Remove(hoadon);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xoá hoá đơn thành công!";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hoadon = await _context.Hoadons.FindAsync(id);
            if (hoadon != null)
                _context.Hoadons.Remove(hoadon);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(string mahoadon)
        {
            if (string.IsNullOrWhiteSpace(mahoadon))
                return BadRequest();

            var hoadon = await _context.Hoadons.FirstOrDefaultAsync(h => h.Mahoadon == mahoadon);

            if (hoadon == null)
                return NotFound();

            hoadon.Ngaythanhtoan = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private bool HoadonExists(string id)
        {
            return _context.Hoadons.Any(e => e.Mahoadon == id);
        }
    }
}
