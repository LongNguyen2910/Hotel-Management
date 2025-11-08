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

        // GET: Ncccungcapthietbis
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;

            var query = _context.Ncccungcapthietbis
                .Include(n => n.ManccNavigation)
                .Include(n => n.MathietbiNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var trimmed = searchString.Trim();
                query = query.Where(n =>
                    EF.Functions.Like(n.Mancc ?? string.Empty, $"%{trimmed}%") ||
                    EF.Functions.Like(n.ManccNavigation.Tennhacungcap ?? string.Empty, $"%{trimmed}%") ||
                    EF.Functions.Like(n.MathietbiNavigation.Tenthietbi ?? string.Empty, $"%{trimmed}%"));
            }

            query = query.OrderBy(n => n.Mancc);

            int pageSize = 10;
            var model = await PaginatedList<Ncccungcapthietbi>.CreateAsync(query, pageNumber ?? 1, pageSize);
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
                Console.WriteLine("❌ ModelState invalid: " + string.Join("; ", errors));

                LoadSelectLists(entity.Mancc, entity.Mathietbi);
                return View(entity);
            }

            try
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm nhà cung cấp thiết bị thành công!";
                Console.WriteLine("✅ Lưu thành công: " + entity.Mancc + " - " + entity.Mathietbi);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx,
                    "Lỗi khi thêm Ncccungcapthietbi (Mancc={Mancc}, Mathietbi={Mathietbi})",
                    entity.Mancc, entity.Mathietbi);

                Console.WriteLine("❌ DbUpdateException: " + dbEx.InnerException?.Message ?? dbEx.Message);

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
