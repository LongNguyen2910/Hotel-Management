using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Hotel_Management.Helpers;

namespace Hotel_Management.Controllers
{
    [Route("~/[controller]/[action]")]
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
        public async Task<IActionResult> Index(int? searchMathietbi, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchMathietbi?.ToString() ?? string.Empty;

            var query = _context.Thietbiduocbaotris
                .AsNoTracking()
                .AsQueryable();

            if (searchMathietbi.HasValue)
            {
                query = query.Where(t => t.Mathietbi == searchMathietbi.Value);
            }

            query = query.OrderByDescending(t => t.Ngaybatdau);

            int pageSize = 10;
            return View(await PaginatedList<Thietbiduocbaotri>.CreateAsync(query, pageNumber ?? 1, pageSize));
        }

        // GET: Thietbiduocbaotris/Create
        public async Task<IActionResult> Create()
        {
            await PopulateThietbiSelectListAsync();
            return View();
        }

        // POST: Thietbiduocbaotris/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ngaybatdau,Ngayketthuc,Mathietbi")] Thietbiduocbaotri entity)
        {
            ModelState.Remove(nameof(entity.Baotri));
            ModelState.Remove("Baotri");

            if (entity.Mathietbi == 0)
            {
                ModelState.AddModelError(nameof(entity.Mathietbi), "Vui lòng chọn thiết bị.");
            }
            if (entity.Ngayketthuc < entity.Ngaybatdau)
            {
                ModelState.AddModelError(nameof(entity.Ngayketthuc), "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    if (kv.Value?.Errors?.Count > 0)
                    {
                        _logger.LogWarning("ModelState error for '{Key}': {Errors}", kv.Key, string.Join(" | ", kv.Value.Errors.Select(e => e.ErrorMessage)));
                    }
                }
                await PopulateThietbiSelectListAsync(entity.Mathietbi);
                return View(entity);
            }

            var tb = await _context.Thietbis.FindAsync(entity.Mathietbi);
            if (tb == null)
            {
                ModelState.AddModelError(nameof(entity.Mathietbi), $"Thiết bị với mã {entity.Mathietbi} không tồn tại.");
                await PopulateThietbiSelectListAsync(entity.Mathietbi);
                return View(entity);
            }

            try
            {
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

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed when creating maintenance record (Thietbi={Id})", entity?.Mathietbi);
                ModelState.AddModelError(string.Empty, "Lỗi khi lưu vào cơ sở dữ liệu: " + (dbEx.InnerException?.Message ?? dbEx.Message));
                await PopulateThietbiSelectListAsync(entity.Mathietbi);
                return View(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when creating maintenance record");
                ModelState.AddModelError(string.Empty, "Đã có lỗi xảy ra: " + ex.Message);
                await PopulateThietbiSelectListAsync(entity.Mathietbi);
                return View(entity);
            }
        }

   
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

        private async Task PopulateThietbiSelectListAsync(int? selectedId = null)
        {
            var items = await _context.Thietbis
                .OrderBy(tb => tb.Mathietbi)
                .Select(tb => new { tb.Mathietbi, Label = tb.Mathietbi + " - " + (tb.Tenthietbi ?? string.Empty) })
                .ToListAsync();

            ViewBag.ThietbiList = new SelectList(items, "Mathietbi", "Label", selectedId);
        }
    }
}