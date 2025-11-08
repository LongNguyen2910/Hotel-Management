using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Hotel_Management.Models;
using Hotel_Management.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Hotel_Management.Controllers
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

        [Authorize(Policy = "CanViewData")]
        // GET: Loaiphongs
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;
            var query = _context.Loaiphongs
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var trimmed = searchString.Trim();
                // search by Tenloaiphong
                query = query.Where(l => EF.Functions.Like(l.Tenloaiphong, $"%{trimmed}%"));
            }

            query = query.OrderBy(l => l.Maloaiphong);

            int pageSize = 10;
            var model = await PaginatedList<Loaiphong>.CreateAsync(query, pageNumber ?? 1, pageSize);

            return View(model);
        }

        // GET: Loaiphongs/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Quản lý khách sạn, Admin")]
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

        [Authorize(Roles = "Quản lý khách sạn, Admin")]
        // GET: Loaiphongs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var loaiphong = await _context.Loaiphongs.FindAsync(id);
            if (loaiphong == null) return NotFound();

            return View(loaiphong);
        }

        [Authorize(Roles = "Quản lý khách sạn, Admin")]
        // POST: Loaiphongs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Maloaiphong,Tenloaiphong,Succhua,Gia")] Loaiphong loaiphong)
        {
            if (id != loaiphong.Maloaiphong) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(loaiphong);
            }

            try
            {
                _context.Update(loaiphong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException dbcex)
            {
                if (!LoaiphongExists(id))
                {
                    return NotFound();
                }

                _logger.LogError(dbcex, "Concurrency error when editing Loaiphong (Maloaiphong={Id})", id);
                ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user. Reload and try again.");
                return View(loaiphong);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed when editing Loaiphong (Maloaiphong={Id})", id);
                ModelState.AddModelError(string.Empty, "Unable to save changes to the database. See logs for details.");
                return View(loaiphong);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when editing Loaiphong (Maloaiphong={Id})", id);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. See logs.");
                return View(loaiphong);
            }
        }

        [Authorize(Roles = "Quản lý khách sạn, Admin")]
        // GET: Loaiphongs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var loaiphong = await _context.Loaiphongs.FirstOrDefaultAsync(m => m.Maloaiphong == id);
            if (loaiphong == null) return NotFound();

            return View(loaiphong);
        }

        [Authorize(Roles = "Quản lý khách sạn, Admin")]
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

        private bool LoaiphongExists(int id)
        {
            return _context.Loaiphongs.Any(e => e.Maloaiphong == id);
        }
    }
}