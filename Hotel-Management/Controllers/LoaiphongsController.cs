using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Hotel_Management.Models;

namespace test.Controllers
{
    public class LoaiphongsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LoaiphongsController> _logger;

        public LoaiphongsController(AppDbContext context, ILogger<LoaiphongsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Loaiphongs
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;
            var query = _context.Loaiphongs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var trimmed = searchString.Trim();
                // search by Tenloaiphong 
                query = query.Where(l => EF.Functions.Like(l.Tenloaiphong, $"%{trimmed}%"));
            }

            var list = await query.ToListAsync();
            return View(list);
        }

        // GET: Loaiphongs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Loaiphongs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Maloaiphong,Tenloaiphong,Succhua,Gia")] Loaiphong loaiphong)
        {
            
            try
            {
                _context.Loaiphongs.Add(loaiphong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed when creating Loaiphong (Maloaiphong={Id})", loaiphong?.Maloaiphong);
                ModelState.AddModelError(string.Empty, "Unable to save changes to the database. See logs for details.");
                return View(loaiphong);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when creating Loaiphong");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. See logs.");
                return View(loaiphong);
            }
        }

        // GET: Loaiphongs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var loaiphong = await _context.Loaiphongs.FirstOrDefaultAsync(m => m.Maloaiphong == id);
            if (loaiphong == null) return NotFound();

            return View(loaiphong);
        }

        // POST: Loaiphongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiphong = await _context.Loaiphongs.FindAsync(id);
            if (loaiphong != null)
            {
                _context.Loaiphongs.Remove(loaiphong);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}