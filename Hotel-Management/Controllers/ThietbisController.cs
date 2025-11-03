using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Hotel_Management.Models;

namespace Hotel_Management.Controllers {   
    public class ThietbisController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ThietbisController> _logger;

        public ThietbisController(AppDbContext context, ILogger<ThietbisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Thietbis
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;

            var query = _context.Thietbis.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var trimmed = searchString.Trim();
                // use EF.Functions.Like for SQL LIKE; case-sensitivity depends on DB collation
                query = query.Where(t => EF.Functions.Like(t.Tenthietbi, $"%{trimmed}%"));
            }

            var list = await query.ToListAsync();
            return View(list);
        }

        // GET: Thietbis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Thietbis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mathietbi,Tenthietbi,Tinhtrang,Maloaithietbi,Tenloaithietbi")] Thietbi thietbi)
        {
            if (!ModelState.IsValid)
            {
                // Log modelstate errors for debugging
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

        // GET: Thietbis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var thietbi = await _context.Thietbis.FirstOrDefaultAsync(m => m.Mathietbi == id);
            if (thietbi == null) return NotFound();

            return View(thietbi);
        }

        // POST: Thietbis/Delete/5
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
