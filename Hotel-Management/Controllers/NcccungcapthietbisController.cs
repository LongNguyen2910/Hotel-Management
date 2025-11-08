using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Hotel_Management.Models;
using Hotel_Management.Helpers;

namespace Hotel_Management.Controllers
{
    public class NcccungcapthietbisController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NcccungcapthietbisController> _logger;

        public NcccungcapthietbisController(AppDbContext context, ILogger<NcccungcapthietbisController> logger)
        {
            _context = context;
            _logger = logger;
        }
        private bool IsAjaxRequest()
   => string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        // GET: Ncccungcapthietbis
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            // Nếu searchString và pageNumber null, lấy từ session
            if (searchString == null && !pageNumber.HasValue)
            {
                var ss = HttpContext.Session.GetString("Ncccungcapthietbis_Search");
                var sp = HttpContext.Session.GetInt32("Ncccungcapthietbis_Page");
                if (!string.IsNullOrEmpty(ss))
                    searchString = ss;
                if (sp.HasValue)
                    pageNumber = sp;
            }

            var trimmed = (searchString ?? string.Empty).Trim();
            ViewData["CurrentFilter"] = searchString ?? string.Empty;

            var query = _context.Ncccungcapthietbis
                .Include(n => n.ManccNavigation)
                .Include(n => n.MathietbiNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                query = query.Where(n =>
                    EF.Functions.Like(n.Mancc ?? string.Empty, $"%{trimmed}%") ||
                    EF.Functions.Like(n.ManccNavigation.Tennhacungcap ?? string.Empty, $"%{trimmed}%") ||
                    EF.Functions.Like(n.MathietbiNavigation.Tenthietbi ?? string.Empty, $"%{trimmed}%"));
            }

            query = query.OrderBy(n => n.Mancc);

            int pageSize = 10;
            var model = await PaginatedList<Ncccungcapthietbi>.CreateAsync(query, pageNumber ?? 1, pageSize);

            // Lưu session
            HttpContext.Session.SetString("Ncccungcapthietbis_Search", searchString ?? string.Empty);
            HttpContext.Session.SetInt32("Ncccungcapthietbis_Page", model.PageIndex);

            if (IsAjaxRequest())
                return PartialView("_NcccungcapthietbisList", model);

            return View(model);
        }

        // GET: Details
        public async Task<IActionResult> Details(string mancc, int mathietbi)
        {
            if (mancc == null) return NotFound();

            var entity = await _context.Ncccungcapthietbis
                .Include(n => n.ManccNavigation)
                .Include(n => n.MathietbiNavigation)
                .FirstOrDefaultAsync(m => m.Mancc == mancc && m.Mathietbi == mathietbi);

            if (entity == null) return NotFound();

            return View(entity);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewData["Mancc"] = new SelectList(_context.Nhacungcaps, "Manhacungcap", "Tennhacungcap");
            ViewData["Mathietbi"] = new SelectList(_context.Thietbis, "Mathietbi", "Tenthietbi");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mancc,Mathietbi,Soluong,Tienthietbi")] Ncccungcapthietbi entity)
        {
            // Log lỗi nếu ModelState invalid
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                _logger.LogError("ModelState invalid: {Errors}", string.Join("; ", errors));
                LoadSelectLists(entity.Mancc, entity.Mathietbi);
                return View(entity);
            }
            try
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm nhà cung cấp thiết bị thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx,
                    "Lỗi khi thêm Ncccungcapthietbi (Mancc={Mancc}, Mathietbi={Mathietbi})",
                    entity.Mancc, entity.Mathietbi);
                ModelState.AddModelError(string.Empty, "Không thể lưu dữ liệu vào cơ sở dữ liệu.");
                LoadSelectLists(entity.Mancc, entity.Mathietbi);
                return View(entity);
            }
        }


        // GET: Edit
        public async Task<IActionResult> Edit(string mancc, int mathietbi)
        {
            if (mancc == null) return NotFound();

            var entity = await _context.Ncccungcapthietbis.FindAsync(mancc, mathietbi);
            if (entity == null) return NotFound();

            ViewData["Mancc"] = new SelectList(_context.Nhacungcaps, "Manhacungcap", "Tennhacungcap", entity.Mancc);
            ViewData["Mathietbi"] = new SelectList(_context.Thietbis, "Mathietbi", "Tenthietbi", entity.Mathietbi);
            return View(entity);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string mancc, int mathietbi, [Bind("Mancc,Mathietbi,Soluong,Tienthietbi")] Ncccungcapthietbi entity)
        {
            if (mancc != entity.Mancc || mathietbi != entity.Mathietbi) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["Mancc"] = new SelectList(_context.Nhacungcaps, "Manhacungcap", "Tennhacungcap", entity.Mancc);
                ViewData["Mathietbi"] = new SelectList(_context.Thietbis, "Mathietbi", "Tenthietbi", entity.Mathietbi);
                return View(entity);
            }

            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thông tin thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(entity.Mancc, entity.Mathietbi)) return NotFound();
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Lỗi khi cập nhật nhà cung cấp thiết bị");
                ModelState.AddModelError("", "Không thể cập nhật dữ liệu.");
                return View(entity);
            }
        }

        // GET: Delete
        public async Task<IActionResult> Delete(string mancc, int mathietbi)
        {
            if (mancc == null) return NotFound();

            var entity = await _context.Ncccungcapthietbis
                .Include(n => n.ManccNavigation)
                .Include(n => n.MathietbiNavigation)
                .FirstOrDefaultAsync(m => m.Mancc == mancc && m.Mathietbi == mathietbi);

            if (entity == null) return NotFound();

            return View(entity);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string mancc, int mathietbi)
        {
            var entity = await _context.Ncccungcapthietbis.FindAsync(mancc, mathietbi);
            if (entity != null)
            {
                _context.Ncccungcapthietbis.Remove(entity);
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "Đã xóa thành công.";
            return RedirectToAction(nameof(Index));
        }
        private void LoadSelectLists(string? selectedMancc = null, int? selectedMathietbi = null)
        {
            ViewData["Mancc"] = new SelectList(_context.Nhacungcaps, "Manhacungcap", "Tennhacungcap", selectedMancc);
            ViewData["Mathietbi"] = new SelectList(_context.Thietbis, "Mathietbi", "Tenthietbi", selectedMathietbi);
        }

        private bool Exists(string mancc, int mathietbi)
        {
            return _context.Ncccungcapthietbis.Any(e => e.Mancc == mancc && e.Mathietbi == mathietbi);
        }
    }
}
