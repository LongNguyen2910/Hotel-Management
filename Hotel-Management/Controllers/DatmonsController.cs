using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hotel_Management.Models;

namespace Hotel_Management.Controllers
{
    [Route("~/[controller]/[action]")]
    public class DatmonsController : Controller
    {
        private readonly AppDbContext _context;

        public DatmonsController(AppDbContext context)
        {
            _context = context;
        }
        // GET: Datmons
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var mons = from h in _context.Mons
                       select h;

            if (!string.IsNullOrEmpty(searchString))
            {
                mons = mons.Where(h =>
                    !string.IsNullOrEmpty(h.Tenmon) &&
                    EF.Functions.Like(h.Tenmon, "%" + searchString + "%")
                );
            }

            var list = await mons.ToListAsync();

            if (!list.Any())
                ViewData["NoResults"] = true;

            return View(list);
        }

        // Cập nhật hoặc tạo hóa đơn
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

            // Tính tổng tiền món
            var tongTienMon = await _context.Khachhangdatmons
                .Include(dm => dm.MamonNavigation)
                .Where(dm => dm.Makhachhang == makhachhang)
                .SumAsync(dm => (dm.MamonNavigation!.Gia ?? 0) * (dm.Soluong ?? 1));

            hoadon.Giamon = tongTienMon;
            hoadon.Ngaylap = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        // Xử lý đặt món
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

            // Lưu từng món
            for (int i = 0; i < mamon.Length; i++)
            {
                string maMon = mamon[i];
                int sl = soluong[i];

                var existing = await _context.Khachhangdatmons
                    .FirstOrDefaultAsync(k =>
                        k.Makhachhang == makhachhang &&
                        k.Mamon == maMon);

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

            // Cập nhật hóa đơn sau khi lưu xong tất cả món
            await CapNhatHoaDonAsync(makhachhang);

            TempData["Success"] = "Đặt món và cập nhật hoá đơn thành công!";
            return RedirectToAction("Index", "Khachhangdatmons");
        }

        // GET: Datmons/Create
        public async Task<IActionResult> Create(string mamon)
        {
            var mon = await _context.Mons.FindAsync(mamon);
            if (mon == null)
                return NotFound();

            ViewBag.Mon = mon;
            ViewBag.Mamon = mamon;
            return View();
        }

        // POST: Datmons/Create
        [HttpPost]
        public IActionResult Create([FromForm] string[] selectedMons)
        {
            if (selectedMons == null || selectedMons.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một món.";
                return RedirectToAction(nameof(Index));
            }

            var mons = _context.Mons
                .Where(m => selectedMons.Contains(m.Mamon))
                .ToList();

            return View(mons);
        }
    }
}
