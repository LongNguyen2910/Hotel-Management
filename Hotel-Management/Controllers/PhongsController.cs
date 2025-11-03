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
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Phongs.Include(p => p.MaloaiphongNavigation);
            return View(await appDbContext.ToListAsync());
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
                if (AnhFile != null && AnhFile.Length > 0)
                {
                    // Thư mục lưu ảnh ngoài wwwroot
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

                    // Tạo tên file an toàn
                    var fileName = Path.GetRandomFileName() + ext;
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await AnhFile.CopyToAsync(stream);
                    }

                    // Lưu tên file hoặc đường dẫn tương đối vào DB
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
            ViewData["Maloaiphong"] = new SelectList(_context.Loaiphongs, "Maloaiphong", "Maloaiphong", phong.Maloaiphong);
            return View(phong);
        }

        // POST: Phongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Maphong,Tenphong,Tinhtrang,Mota,Maloaiphong,Anhphong")] Phong phong)
        {
            if (id != phong.Maphong)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongExists(phong.Maphong))
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
            ViewData["Maloaiphong"] = new SelectList(_context.Loaiphongs, "Maloaiphong", "Maloaiphong", phong.Maloaiphong);
            return View(phong);
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
        }

        private bool PhongExists(string id)
        {
            return _context.Phongs.Any(e => e.Maphong == id);
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
