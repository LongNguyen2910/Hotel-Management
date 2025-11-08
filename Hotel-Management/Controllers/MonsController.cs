using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Management.Models;
using Hotel_Management.Helpers;

namespace Hotel_Management.Controllers
{
    [Route("~/[controller]/[action]")]
    public class MonsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public MonsController(AppDbContext context , IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Mons
        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 10;
            return View(await PaginatedList<Mon>.CreateAsync(_context.Mons.AsNoTracking(),pageNumber ?? 1, pageSize));
        }

        // GET: Mons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mamon,Tenmon,Gia,Anhmon")] Mon mon, IFormFile? AnhFile)
        {
            if (MonExists(mon.Mamon))
            {
                ModelState.AddModelError("Mamon", "Mã món đã tồn tại. Vui lòng sử dụng mã khác.");
            }


            if (ModelState.IsValid)
            {
                if (AnhFile != null && AnhFile.Length > 0)
                {
                    // Thư mục lưu ảnh ngoài wwwroot
                    var folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Mon");
                    Directory.CreateDirectory(folderPath);

                    //Validate file type
                    var allowedExts = new[] { ".jpg", ".jpeg", ".png" };
                    var ext = Path.GetExtension(AnhFile.FileName).ToLowerInvariant();
                    if (!allowedExts.Contains(ext))
                    {
                        ModelState.AddModelError("AnhFile", "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPG, PNG.");
                        return View(mon);
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
                    mon.Anhmon = fileName;
                }
                _context.Add(mon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mon);
        }

        // GET: Mons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mon = await _context.Mons.FindAsync(id);
            if (mon == null)
            {
                return NotFound();
            }
            return View(mon);
        }

        // POST: Mons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Mamon,Tenmon,Gia,Anhmon")] Mon mon, IFormFile? AnhFile)
        {
            if (id != mon.Mamon)
            {
                return NotFound();
            }

            var originalMon = await _context.Mons.AsNoTracking().FirstOrDefaultAsync(m => m.Mamon == id);
            if (originalMon == null)
            {
                return NotFound();
            }
            string oldAnhmon = originalMon.Anhmon;

            if (ModelState.IsValid)
            {
                string newAnhmon = oldAnhmon;

                if (AnhFile != null && AnhFile.Length > 0)
                {
                    // (Copy logic xử lý file từ action Create)
                    var folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Mon");
                    Directory.CreateDirectory(folderPath);

                    // Validate file type
                    var allowedExts = new[] { ".jpg", ".jpeg", ".png" };
                    var ext = Path.GetExtension(AnhFile.FileName).ToLowerInvariant();
                    if (!allowedExts.Contains(ext))
                    {
                        ModelState.AddModelError("AnhFile", "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPG, PNG.");
                        mon.Anhmon = oldAnhmon; // Gán lại ảnh cũ để hiển thị nếu lỗi
                        return View(mon);
                    }

                    // Tạo tên file an toàn
                    string fileName, filePath;
                    do
                    {
                        fileName = Path.GetRandomFileName() + ext;
                        filePath = Path.Combine(folderPath, fileName);
                    }
                    while (System.IO.File.Exists(filePath));

                    // Lưu file mới
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await AnhFile.CopyToAsync(stream);
                    }

                    // Cập nhật tên file mới
                    newAnhmon = fileName;
                }

                // Gán tên file (cũ hoặc mới) vào đối tượng 'mon' trước khi Update
                mon.Anhmon = newAnhmon;
                try
                {
                    _context.Update(mon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonExists(mon.Mamon))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                if (newAnhmon != oldAnhmon && !string.IsNullOrEmpty(oldAnhmon))
                {
                    var oldFilePath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Mon", oldAnhmon);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            mon.Anhmon = oldAnhmon;
            return View(mon);
        }

        // GET: Mons/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mon = await _context.Mons
                .FirstOrDefaultAsync(m => m.Mamon == id);
            if (mon == null)
            {
                return NotFound();
            }

            return View(mon);
        }

        // POST: Mons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var mon = await _context.Mons
                .Include(m => m.Thucdons)
                .FirstOrDefaultAsync(m => m.Mamon == id);
            if (mon == null)
            {
                return NotFound();
            }

            mon.Thucdons.Clear();

            if (!string.IsNullOrEmpty(mon.Anhmon))
            {
                var folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Mon");
                var filePath = Path.Combine(folderPath, mon.Anhmon);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Mons.Remove(mon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonExists(string id)
        {
            return _context.Mons.Where(e => e.Mamon == id).Count() > 0;
        }

        public IActionResult GetImage(string id)
        {
            var mon = _context.Mons
                .AsNoTracking()
                .FirstOrDefault(p => p.Mamon == id);

            if (mon == null || string.IsNullOrEmpty(mon.Anhmon))
                return NotFound();

            // Xác định đường dẫn vật lý tới file
            var folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Mon");
            var filePath = Path.Combine(folderPath, mon.Anhmon);

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
