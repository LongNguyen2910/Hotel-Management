using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Hotel_Management.Models;

namespace Hotel_Management.Controllers
{
    public class ThietbiduocbaotrisController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ThietbiduocbaotrisController> _logger;

        public ThietbiduocbaotrisController(AppDbContext context, ILogger<ThietbiduocbaotrisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Thietbiduocbaotris
        public async Task<IActionResult> Index(int? searchMathietbi)
        {
            ViewData["CurrentFilter"] = searchMathietbi?.ToString() ?? string.Empty;

            var query = _context.Thietbiduocbaotris.AsQueryable();

            if (searchMathietbi.HasValue)
            {
                query = query.Where(t => t.Mathietbi == searchMathietbi.Value);
            }

            var list = await query.OrderByDescending(t => t.Ngaybatdau).ToListAsync();
            return View(list);
        }

        // GET: Thietbiduocbaotris/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Thietbiduocbaotris/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ngaybatdau,Ngayketthuc,Mathietbi")] Thietbiduocbaotri entity)
        {
            // log modelstate errors for debugging
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    if (kv.Value?.Errors?.Count > 0)
                    {
                        _logger.LogWarning("ModelState error for '{Key}': {Errors}", kv.Key, string.Join(" | ", kv.Value.Errors.Select(e => e.ErrorMessage)));
                    }
                }
                return View(entity);
            }

            // basic validation: end date must be >= start date
            if (entity.Ngayketthuc < entity.Ngaybatdau)
            {
                ModelState.AddModelError(nameof(entity.Ngayketthuc), "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
                return View(entity);
            }

            // verify Thietbi exists (if required)
            var tb = await _context.Thietbis.FindAsync(entity.Mathietbi);
            if (tb == null)
            {
                ModelState.AddModelError(nameof(entity.Mathietbi), $"Thiết bị với mã {entity.Mathietbi} không tồn tại.");
                return View(entity);
            }

            try
            {
                // Ensure a Baotri row exists for the date pair (FK from Thietbiduocbaotri -> Baotri)
                var existingBaotri = await _context.Baotris.FindAsync(entity.Ngaybatdau, entity.Ngayketthuc);
                if (existingBaotri == null)
                {
                    var newBaotri = new Baotri
                    {
                        Ngaybatdau = entity.Ngaybatdau,
                        Ngayketthuc = entity.Ngayketthuc,
                        Tienbaotri = 0m,
                        Trangthai = true,
                    };
                    _context.Baotris.Add(newBaotri);
                }

                _context.Thietbiduocbaotris.Add(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed when creating maintenance record (Thietbi={Id})", entity?.Mathietbi);
                ModelState.AddModelError(string.Empty, "Lỗi khi lưu vào cơ sở dữ liệu: " + (dbEx.InnerException?.Message ?? dbEx.Message));
                return View(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when creating maintenance record");
                ModelState.AddModelError(string.Empty, "Đã có lỗi xảy ra: " + ex.Message);
                return View(entity);
            }
        }

        // GET: Thietbiduocbaotris/Delete?ngaybatdauticks=...&ngayketthucticks=...&mathietbi=...
        public async Task<IActionResult> Delete(long? ngaybatdauticks, long? ngayketthucticks, int? mathietbi)
        {
            if (!ngaybatdauticks.HasValue || !ngayketthucticks.HasValue || !mathietbi.HasValue)
                return NotFound();

            var ngaybd = new DateTime(ngaybatdauticks.Value);
            var ngaykt = new DateTime(ngayketthucticks.Value);

            var entity = await _context.Thietbiduocbaotris
                .FirstOrDefaultAsync(e => e.Ngaybatdau == ngaybd && e.Ngayketthuc == ngaykt && e.Mathietbi == mathietbi.Value);

            if (entity == null) return NotFound();

            return View(entity);
        }

        // POST: Thietbiduocbaotris/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long ngaybatdauticks, long ngayketthucticks, int mathietbi)
        {
            var ngaybd = new DateTime(ngaybatdauticks);
            var ngaykt = new DateTime(ngayketthucticks);

            var entity = await _context.Thietbiduocbaotris
                .FirstOrDefaultAsync(e => e.Ngaybatdau == ngaybd && e.Ngayketthuc == ngaykt && e.Mathietbi == mathietbi);

            if (entity != null)
            {
                _context.Thietbiduocbaotris.Remove(entity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}