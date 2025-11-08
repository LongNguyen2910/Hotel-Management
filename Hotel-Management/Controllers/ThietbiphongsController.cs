using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Hotel_Management.Helpers;

namespace Hotel_Management.Controllers
{
    [Route("~/[controller]/[action]")]
    public class ThietbiphongsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ThietbiphongsController> _logger;

        public ThietbiphongsController(AppDbContext context, ILogger<ThietbiphongsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Thietbiphongs
        // Search by mã thiết bị (Mathietbi) and return view models so the view's model type matches
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;
            var trimmed = (searchString ?? string.Empty).Trim();

            var query = _context.Thietbis
                                .AsNoTracking()
                                .Include(t => t.Maphongs)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                if (int.TryParse(trimmed, out var id))
                {
                    query = query.Where(t => t.Mathietbi == id);
                }
                else
                {
                    query = query.Where(t => false);
                }
            }

            query = query.OrderBy(t => t.Mathietbi);

            int pageSize = 10;
            var model = await PaginatedList<Thietbi>.CreateAsync(query, pageNumber ?? 1, pageSize);

            return View(model);
        }

        // GET: Thietbiphongs/Create
        public IActionResult Create()
        {
            ViewData["Maphong"] = new SelectList(_context.Loaiphongs, "Maphong", "Tenphong");
            return View();
        }

        // POST: Thietbiphongs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mathietbi,Tenthietbi,Tinhtrang")] Thietbi thietbi)
        {
            ViewData["Maphong"] = new SelectList(_context.Loaiphongs, "Maphong", "Tenphong", thietbi.Maphongs);
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    if (kv.Value?.Errors?.Count > 0)
                    {
                        _logger.LogWarning("ModelState error for '{Key}': {Errors}", kv.Key, string.Join(" | ", kv.Value.Errors));
                    }
                }
                return View(thietbi);
            }

            try
            {
                _context.Thietbis.Add(thietbi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed when creating Thietbi (Mathietbi={Id})", thietbi?.Mathietbi);
                ModelState.AddModelError(string.Empty, "Unable to save changes to the database. See logs for details.");
                return View(thietbi);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when creating Thietbi");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. See logs.");
                return View(thietbi);
            }
        }

        // GET: Thietbiphongs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var thietbi = await _context.Thietbis.FirstOrDefaultAsync(m => m.Mathietbi == id);
            if (thietbi == null) return NotFound();

            return View(thietbi);
        }

        // POST: Thietbiphongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thietbi = await _context.Thietbis.FindAsync(id);
            if (thietbi != null)
            {
                _context.Thietbis.Remove(thietbi);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}