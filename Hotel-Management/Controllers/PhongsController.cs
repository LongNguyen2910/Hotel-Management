using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Hotel_Management.Controllers
{
    public class PhongsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PhongsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Phongs
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString ?? string.Empty;
            var trimmed = (searchString ?? string.Empty).Trim().ToUpper();

            var query = _context.Phongs
                .Include(p => p.MaloaiphongNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                // use SQL LIKE 
                query = query.Where(p => EF.Functions.Like(p.Tenphong, $"%{trimmed}%"));
            }

            var list = await query.ToListAsync();
            if (!list.Any())
            {
                ViewData["NoResults"] = true;
            }
            return View(list);
        }

        // GET: Phongs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phong = await _context.Phongs
                .Include(p => p.MaloaiphongNavigation)
                .FirstOrDefaultAsync(m => m.Maphong == id);
            if (phong == null)
            {
                return NotFound();
            }

            return View(phong);
        }

        // GET: Phongs/Create
        public IActionResult Create()
        {
            ViewData["Maloaiphong"] = new SelectList(_context.Loaiphongs, "Maloaiphong", "Tenloaiphong");
            return View();
        }

        // POST: Phongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Maphong,Tenphong,Tinhtrang,Mota,Maloaiphong,Anhphong")] Phong phong, IFormFile? AnhFile)
        {
            if (ModelState.IsValid)
            {

                if (_context.Phongs.Count(k => k.Maphong == phong.Maphong) > 0)
                {
                    ModelState.AddModelError("Maphong", "Mã phòng đã tồn tại");
                    return View(phong);
                }

                if (AnhFile != null && AnhFile.Length > 0)
                {
                    
                    var folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Phong");
                    Directory.CreateDirectory(folderPath);

                    //Validate file type
                    var allowedExts = new[] { ".jpg", ".jpeg", ".png" };
                    var ext = Path.GetExtension(AnhFile.FileName).ToLowerInvariant();
                    if (!allowedExts.Contains(ext))
                    {
                        ModelState.AddModelError("AnhFile", "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPG, PNG.");
                        return View(phong);
                    }

                    // Tạo tên file an 
                    string fileName, filePath;
                    do
                    {
                        fileName = Path.GetRandomFileName() + ext;
                        filePath = Path.Combine(folderPath, fileName);
                    }
                    while (System.IO.File.Exists(filePath));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await AnhFile.CopyToAsync(stream);
                    }

                    // Lưu tên file vào DB
                    phong.Anhphong = fileName;
                }

                _context.Add(phong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Maloaiphong"] = new SelectList(_context.Loaiphongs, "Maloaiphong", "Tenloaiphong", phong.Maloaiphong);
            return View(phong);
        }

        // GET: Phongs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null)
            {
                return NotFound();
            }
            ViewData["Maloaiphong"] = new SelectList(_context.Loaiphongs, "Maloaiphong", "Tenloaiphong", phong.Maloaiphong);
            return View(phong);
        }

        // POST: Phongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Maphong,Tenphong,Tinhtrang,Mota,Maloaiphong,Anhphong")] Phong phong, IFormFile? AnhFile)
        {
            if (id != phong.Maphong)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["Maloaiphong"] = new SelectList(_context.Loaiphongs, "Maloaiphong", "Tenloaiphong", phong.Maloaiphong);
                return View(phong);
            }

            // Load current record (to keep or remove the old file)
            var existing = await _context.Phongs.AsNoTracking().FirstOrDefaultAsync(p => p.Maphong == id);
            if (existing == null) return NotFound();

            // If a new image is uploaded, validate and save it
            if (AnhFile != null && AnhFile.Length > 0)
            {
                var allowedExts = new[] { ".jpg", ".jpeg", ".png" };
                var ext = Path.GetExtension(AnhFile.FileName).ToLowerInvariant();
                if (!allowedExts.Contains(ext))
                {
                    ModelState.AddModelError("AnhFile", "Invalid image format. Only JPG/PNG are allowed.");
                    ViewData["Maloaiphong"] = new SelectList(_context.Loaiphongs, "Maloaiphong", "Maloaiphong", phong.Maloaiphong);
                    return View(phong);
                }

                var folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Phong");
                if (System.IO.File.Exists(Path.Combine(folderPath, existing.Anhphong)))
                {
                    System.IO.File.Delete(Path.Combine(folderPath, existing.Anhphong));
                }
                Directory.CreateDirectory(folderPath);
                string fileName, filePath;
                do
                {
                    fileName = Path.GetRandomFileName() + ext;
                    filePath = Path.Combine(folderPath, fileName);
                }
                while (System.IO.File.Exists(filePath));
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AnhFile.CopyToAsync(stream);
                }

                // Optionally delete old file
                if (!string.IsNullOrEmpty(existing.Anhphong))
                {
                    var oldPath = Path.Combine(folderPath, existing.Anhphong);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                phong.Anhphong = fileName; // store the new file name in DB
            }
            else
            {
                // No new image selected: keep the current file name
                phong.Anhphong = existing.Anhphong;
            }

            try
            {
                _context.Update(phong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Phongs.Any(e => e.Maphong == phong.Maphong))
                    return NotFound();
                throw;
            }
        }

        // GET: Phongs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phong = await _context.Phongs
                .Include(p => p.MaloaiphongNavigation)
                .FirstOrDefaultAsync(m => m.Maphong == id);
            if (phong == null)
            {
                return NotFound();
            }

            return View(phong);
        }

        // POST: Phongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong != null)
            {
                _context.Phongs.Remove(phong);
            }
            try
            {
                if (!string.IsNullOrEmpty(phong.Anhphong))
                {
                    var folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Phong");
                    var filePath = Path.Combine(folderPath, phong.Anhphong);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } catch (DbUpdateConcurrencyException)
            {
                if (!_context.Phongs.Any(e => e.Maphong == phong.Maphong))
                    return NotFound();
                throw;
            }
        }

        public IActionResult GetImage(string id)
        {
            var phong = _context.Phongs
                .AsNoTracking()
                .FirstOrDefault(p => p.Maphong == id);

            if (phong == null || string.IsNullOrEmpty(phong.Anhphong))
                return NotFound();

            // Xác định đường dẫn vật lý tới file
            var folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Phong");
            var filePath = Path.Combine(folderPath, phong.Anhphong);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            // Xác định content-type theo đuôi file
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "image.jpeg"
            };

            var imageBytes = System.IO.File.ReadAllBytes(filePath);
            return File(imageBytes, contentType);
        }
    }
}
