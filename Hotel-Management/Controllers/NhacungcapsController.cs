using Hotel_Management.Helpers;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Management.Controllers
{
    public class NhacungcapsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NhacungcapsController> _logger;

        public NhacungcapsController(AppDbContext context, ILogger<NhacungcapsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        private bool IsAjaxRequest()
           => string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        // GET: Nhacungcaps
        [Authorize(Policy = "CanViewData")]
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            // Lấy lại từ session nếu searchString hoặc pageNumber null
            if (searchString == null && !pageNumber.HasValue)
            {
                var ss = HttpContext.Session.GetString("Nhacungcaps_Search");
                var sp = HttpContext.Session.GetInt32("Nhacungcaps_Page");
                if (!string.IsNullOrEmpty(ss))
                    searchString = ss;
                if (sp.HasValue)
                    pageNumber = sp;
            }
            var trimmed = (searchString ?? string.Empty).Trim();
            ViewData["CurrentFilter"] = searchString ?? string.Empty;
            var query = _context.Set<Nhacungcap>().AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                query = query.Where(n =>
                    EF.Functions.Like(n.Manhacungcap ?? string.Empty, $"%{trimmed}%") ||
                    EF.Functions.Like(n.Tennhacungcap ?? string.Empty, $"%{trimmed}%"));
            }
            query = query.OrderBy(n => n.Manhacungcap);

            int pageSize = 10;
            var model = await PaginatedList<Nhacungcap>.CreateAsync(query, pageNumber ?? 1, pageSize);
            // Lưu lại session / trang
            HttpContext.Session.SetString("Nhacungcaps_Search", searchString ?? string.Empty);
            HttpContext.Session.SetInt32("Nhacungcaps_Page", model.PageIndex);
            if (IsAjaxRequest())
                return PartialView("_NhacungcapsList", model);

            return View(model);
        }

        // GET: Nhacungcaps/Details/5
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
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
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
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
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Set<Nhacungcap>().FindAsync(id);
            if (entity == null) return NotFound();

            return View(entity);
        }

        // POST: Nhacungcaps/Edit/5
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
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
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Set<Nhacungcap>().FirstOrDefaultAsync(m => m.Manhacungcap == id);
            if (entity == null) return NotFound();

            return View(entity);
        }

        // POST: Nhacungcaps/Delete/5
        [Authorize(Roles = "Admin, Quản lý khách sạn")]
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