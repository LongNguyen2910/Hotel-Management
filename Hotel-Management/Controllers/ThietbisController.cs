using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Management.Models;
using Hotel_Management.Helpers;
using Microsoft.Extensions.Logging;

namespace Hotel_Management.Controllers
{
    public class ThietbisController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ThietbisController> _logger;

        public ThietbisController(AppDbContext context, ILogger<ThietbisController> logger)
        {
            _context = context;
            _logger = logger;
        }
        private bool IsAjaxRequest()
=> string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        // GET: Thietbis
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            // Lấy từ session nếu không có searchString/pageNumber
            if (searchString == null && !pageNumber.HasValue)
            {
                var ss = HttpContext.Session.GetString("Thietbis_Search");
                var sp = HttpContext.Session.GetInt32("Thietbis_Page");
                if (!string.IsNullOrEmpty(ss))
                    searchString = ss;
                if (sp.HasValue)
                    pageNumber = sp;
            }

            var trimmed = (searchString ?? string.Empty).Trim();
            ViewData["CurrentFilter"] = searchString ?? string.Empty;

            var query = _context.Thietbis
                .Include(t => t.MaloaithietbiNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                query = query.Where(t => EF.Functions.Like(t.Tenthietbi, $"%{trimmed}%"));
            }

            query = query.OrderBy(t => t.Mathietbi);

            int pageSize = 10;
            var model = await PaginatedList<Thietbi>.CreateAsync(query, pageNumber ?? 1, pageSize);

            // Lưu session để nhớ trạng thái tìm kiếm / trang
            HttpContext.Session.SetString("Thietbis_Search", searchString ?? string.Empty);
            HttpContext.Session.SetInt32("Thietbis_Page", model.PageIndex);
            if (IsAjaxRequest())
                return PartialView("_ThietbisList", model);

            return View(model);
        }

        // GET: Thietbis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thietbi = await _context.Thietbis
                .Include(t => t.MaloaithietbiNavigation)
                .FirstOrDefaultAsync(m => m.Mathietbi == id);
            if (thietbi == null)
            {
                return NotFound();
            }

            return View(thietbi);
        }

        // GET: Thietbis/Create
        public IActionResult Create()
        {
            ViewData["Maloaithietbi"] = new SelectList(_context.Loaithietbis, "Maloaithietbi", "Tenloaithietbi");
            return View();
        }

        // POST: Thietbis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mathietbi,Tenthietbi,Tinhtrang,Maloaithietbi")] Thietbi thietbi)
        {
            ViewData["Maloaithietbi"] = new SelectList(_context.Loaithietbis, "Maloaithietbi", "Tenloaithietbi", thietbi.Maloaithietbi);
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

        // GET: Thietbis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thietbi = await _context.Thietbis.FindAsync(id);
            if (thietbi == null)
            {
                return NotFound();
            }
            ViewData["Maloaithietbi"] = new SelectList(_context.Loaithietbis, "Maloaithietbi", "Maloaithietbi", thietbi.Maloaithietbi);
            return View(thietbi);
        }

        // POST: Thietbis/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Mathietbi,Tenthietbi,Tinhtrang,Maloaithietbi")] Thietbi thietbi)
        {
            if (id != thietbi.Mathietbi)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thietbi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThietbiExists(thietbi.Mathietbi))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Maloaithietbi"] = new SelectList(_context.Loaithietbis, "Maloaithietbi", "Maloaithietbi", thietbi.Maloaithietbi);
            return View(thietbi);
        }

        // GET: Thietbis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thietbi = await _context.Thietbis
                .Include(t => t.MaloaithietbiNavigation)
                .FirstOrDefaultAsync(m => m.Mathietbi == id);
            if (thietbi == null)
            {
                return NotFound();
            }

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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThietbiExists(int id)
        {
            return _context.Thietbis.Any(e => e.Mathietbi == id);
        }
    }
}
