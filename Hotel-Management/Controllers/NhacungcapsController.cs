using Hotel_Management.Models;
using Hotel_Management.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Hotel_Management.Controllers
{
    [Route("~/[controller]/[action]")]
    public class NhacungcapsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NhacungcapsController> _logger;

        public NhacungcapsController(AppDbContext context, ILogger<NhacungcapsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Nhacungcaps
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;

            var query = _context.Set<Nhacungcap>().AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var trimmed = searchString.Trim();
                query = query.Where(n =>
                    EF.Functions.Like(n.Manhacungcap ?? string.Empty, $"%{trimmed}%")
                    || EF.Functions.Like(n.Tennhacungcap ?? string.Empty, $"%{trimmed}%"));
            }

            query = query.OrderBy(n => n.Manhacungcap);

            int pageSize = 10;
            var model = await PaginatedList<Nhacungcap>.CreateAsync(query, pageNumber ?? 1, pageSize);
            return View(model);
        }

        // GET: Nhacungcaps/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Set<Nhacungcap>().FirstOrDefaultAsync(m => m.Manhacungcap == id);
            if (entity == null) return NotFound();

            return View(entity);
        }

        // GET: Nhacungcaps/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nhacungcaps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Manhacungcap,Tennhacungcap,Diachi,Sodienthoai,Email")] Nhacungcap nhacungcap)
        {
            if (!ModelState.IsValid)
            {
                return View(nhacungcap);
            }

            try
            {
                _context.Add(nhacungcap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Unable to create Nhacungcap (Manhacungcap={Id})", nhacungcap?.Manhacungcap);
                ModelState.AddModelError(string.Empty, "Lỗi khi lưu cơ sở dữ liệu.");
                return View(nhacungcap);
            }
        }

        // GET: Nhacungcaps/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Set<Nhacungcap>().FindAsync(id);
            if (entity == null) return NotFound();

            return View(entity);
        }

        // POST: Nhacungcaps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Manhacungcap,Tennhacungcap,Diachi,Sodienthoai,Email")] Nhacungcap nhacungcap)
        {
            if (id != nhacungcap.Manhacungcap) return NotFound();

            if (!ModelState.IsValid) return View(nhacungcap);

            try
            {
                _context.Update(nhacungcap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Set<Nhacungcap>().Any(e => e.Manhacungcap == nhacungcap.Manhacungcap))
                    return NotFound();
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Unable to edit Nhacungcap (Manhacungcap={Id})", nhacungcap?.Manhacungcap);
                ModelState.AddModelError(string.Empty, "Lỗi khi lưu cơ sở dữ liệu.");
                return View(nhacungcap);
            }
        }

        // GET: Nhacungcaps/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Set<Nhacungcap>().FirstOrDefaultAsync(m => m.Manhacungcap == id);
            if (entity == null) return NotFound();

            return View(entity);
        }

        // POST: Nhacungcaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var entity = await _context.Set<Nhacungcap>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<Nhacungcap>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}