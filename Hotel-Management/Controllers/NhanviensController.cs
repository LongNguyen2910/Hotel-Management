using Hotel_Management.Helpers;
using Hotel_Management.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Management.Models;
using Hotel_Management.Helpers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hotel_Management.Controllers
{
    public class NhanviensController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public NhanviensController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: NhanVien
        [Authorize(Policy = "CanViewData")]
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            var modelContext = _context.Nhanviens.OrderBy(n => n.Hoten).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                modelContext = modelContext.Where(n => n.Hoten.ToLower().Contains(searchString.ToLower()));
            }

            ViewData["CurrentFilter"] = searchString;
            int pageSize = 10;
            var paginatedList = await PaginatedList<Nhanvien>.CreateAsync(modelContext.AsNoTracking(), pageNumber ?? 1, pageSize);

            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                // Nếu là AJAX, chỉ trả về phần HTML của bảng
                return PartialView("NhanviensTable", paginatedList);
            }

            ViewBag.Trangthai = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Nghỉ việc", Value = "0" },
                new SelectListItem { Text = "Đang làm", Value = "1" }
            };

            return View(paginatedList);
        }
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        // GET: NhanVien/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens
                .Include(n => n.MabophanNavigation)
                .Include(n => n.TenchucvuNavigation)
                .Include(n => n.Nhanvienlamcas)
                    .ThenInclude(nl => nl.MacalamviecNavigation)
                .FirstOrDefaultAsync(m => m.Manv == id);
            if (nhanvien == null)
            {
                return NotFound();
            }

            return View(nhanvien);
        }

        // GET: NhanVien/Create
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        public IActionResult Create()
        {
            ViewData["Mabophan"] = new SelectList(_context.Bophans, "Mabophan", "Tenbophan");
            ViewData["Tenchucvu"] = new SelectList(_context.Chucvus, "Tenchucvu", "Tenchucvu");
            ViewBag.AllCalamviecs = _context.Calamviecs.ToList();
            ViewData["Trangthai"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Nghỉ việc", Value = "0" },
                new SelectListItem { Text = "Đang làm", Value = "1" }
            }, "Value", "Text");
            return View();
        }

        // POST: NhanVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Manv,Hoten,Gioitinh,Ngaysinh,Sodienthoai,Cccd,Ngayvaolam,Trangthai,Mabophan,Tenchucvu,Nhanvienlamcas,AnhNhanVien")] 
            Nhanvien nhanvien, string[] selectedCalamviec, IFormFile? AnhFile)
        {
            if (NhanvienExists(nhanvien.Manv))
            {
                
                ModelState.AddModelError("Manv", "Mã nhân viên này đã tồn tại. Vui lòng chọn mã khác.");
            }

            if (ModelState.IsValid)
            {
                if (AnhFile != null && AnhFile.Length >  0)
                {
                    // Thư mục lưu ảnh ngoài wwwroot
                    var folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Nhanviens");
                    Directory.CreateDirectory(folderPath);

                    //Validate file type
                    var allowedExts = new[] { ".jpg", ".jpeg", ".png" };
                    var ext = Path.GetExtension(AnhFile.FileName).ToLowerInvariant();
                    if (!allowedExts.Contains(ext))
                    {
                        ModelState.AddModelError("AnhFile", "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPG, PNG.");
                        return View(nhanvien);
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
                    nhanvien.AnhNhanVien = fileName;
                }
                _context.Add(nhanvien);

                // Xử lý thêm nhiều ca làm việc
                if (selectedCalamviec != null)
                {
                    foreach (var caId in selectedCalamviec)
                    {
                        nhanvien.Nhanvienlamcas.Add(new Nhanvienlamca 
                        { 
                            Manv = nhanvien.Manv, 
                            Macalamviec = caId 
                        });
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Mabophan"] = new SelectList(_context.Bophans, "Mabophan", "Mabophan", nhanvien.Mabophan);
            ViewData["Tenchucvu"] = new SelectList(_context.Chucvus, "Tenchucvu", "Tenchucvu", nhanvien.Tenchucvu);
            ViewBag.AllCalamviecs = await _context.Calamviecs.ToListAsync();
            ViewBag.Trangthai = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Nghỉ việc", Value = "0" },
                new SelectListItem { Text = "Đang làm", Value = "1" }
            };
            return View(nhanvien);
        }
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        // GET: NhanVien/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens
                .Include(n => n.Nhanvienlamcas) 
                .FirstOrDefaultAsync(n => n.Manv == id);
            if (nhanvien == null)
            {
                return NotFound();
            }
            ViewData["Mabophan"] = new SelectList(_context.Bophans, "Mabophan", "Tenbophan", nhanvien.Mabophan);
            ViewData["Tenchucvu"] = new SelectList(_context.Chucvus, "Tenchucvu", "Tenchucvu", nhanvien.Tenchucvu);

            ViewBag.AllCalamviecs = await _context.Calamviecs.ToListAsync();
            ViewBag.Trangthai = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Nghỉ việc", Value = "0" },
                new SelectListItem { Text = "Đang làm", Value = "1" }
            };
            return View(nhanvien);
        }

        // POST: NhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, 
            [Bind("Manv,Hoten,Gioitinh,Ngaysinh,Sodienthoai,Cccd,Ngayvaolam,Trangthai,Mabophan,Tenchucvu,Nhanvienlamcas,AnhNhanVien")] 
            Nhanvien nhanvien, string[] selectedCalamviec, IFormFile? AnhFile)
        {
            if (id != nhanvien.Manv)
            {
                return NotFound();
            }

            var nhanvienToUpdate = await _context.Nhanviens
                .Include(n => n.Nhanvienlamcas)
                .FirstOrDefaultAsync(n => n.Manv == id);

            var originalNV = await _context.Nhanviens.AsNoTracking().FirstOrDefaultAsync(m => m.Manv == id);
            if (originalNV == null)
            {
                return NotFound();
            }
            string oldAnhNhanVien = originalNV.AnhNhanVien;

            if (nhanvienToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string newAnhNhanVien = oldAnhNhanVien;

                if (AnhFile != null && AnhFile.Length > 0)
                {
                    // (Copy logic xử lý file từ action Create)
                    var folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Nhanviens");
                    Directory.CreateDirectory(folderPath);

                    // Validate file type
                    var allowedExts = new[] { ".jpg", ".jpeg", ".png" };
                    var ext = Path.GetExtension(AnhFile.FileName).ToLowerInvariant();
                    if (!allowedExts.Contains(ext))
                    {
                        ModelState.AddModelError("AnhFile", "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPG, PNG.");
                        nhanvien.AnhNhanVien = oldAnhNhanVien; // Gán lại ảnh cũ để hiển thị nếu lỗi
                        return View(nhanvien);
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
                    newAnhNhanVien = fileName;
                }

                // Gán tên file (cũ hoặc mới) vào đối tượng 'mon' trước khi Update
                nhanvien.AnhNhanVien = newAnhNhanVien;
                try
                {
                    nhanvienToUpdate.Hoten = nhanvien.Hoten;
                    nhanvienToUpdate.Gioitinh = nhanvien.Gioitinh;
                    nhanvienToUpdate.Ngaysinh = nhanvien.Ngaysinh;
                    nhanvienToUpdate.Sodienthoai = nhanvien.Sodienthoai;
                    nhanvienToUpdate.Cccd = nhanvien.Cccd;
                    nhanvienToUpdate.Ngayvaolam = nhanvien.Ngayvaolam;
                    nhanvienToUpdate.Trangthai = nhanvien.Trangthai;
                    nhanvienToUpdate.Mabophan = nhanvien.Mabophan;
                    nhanvienToUpdate.Tenchucvu = nhanvien.Tenchucvu;
                    nhanvienToUpdate.AnhNhanVien = newAnhNhanVien;

                    nhanvienToUpdate.Nhanvienlamcas.Clear();

                    // 2. Thêm lại các món mới được chọn
                    if (selectedCalamviec != null)
                    {
                        foreach (var caId in selectedCalamviec)
                        {
                            // Tìm món ăn trong DB
                            nhanvienToUpdate.Nhanvienlamcas.Add(
                                new Nhanvienlamca 
                                { 
                                    Manv = id, Macalamviec = caId 
                                });

                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanvienExists(nhanvien.Manv))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (newAnhNhanVien != oldAnhNhanVien && !string.IsNullOrEmpty(oldAnhNhanVien))
                {
                    var oldFilePath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Nhanviens", oldAnhNhanVien);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Mabophan"] = new SelectList(_context.Bophans, "Mabophan", "Tenbophan", nhanvien.Mabophan);
            ViewData["Tenchucvu"] = new SelectList(_context.Chucvus, "Tenchucvu", "Tenchucvu", nhanvien.Tenchucvu);
            ViewBag.AllCalamviecs = await _context.Calamviecs.ToListAsync();
            ViewBag.Trangthai = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Nghỉ việc", Value = "0" },
                new SelectListItem { Text = "Đang làm", Value = "1" }
            };
            nhanvien.AnhNhanVien = oldAnhNhanVien;
            return View(nhanvien);
        }
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        public async Task<IActionResult> EditPhoto(string id)
        {
            if (id == null) return NotFound();

            var nhanvien = await _context.Nhanviens.FindAsync(id);
            if (nhanvien == null) return NotFound();

            return View(nhanvien);
        }
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        // POST: NhanVien/EditPhoto/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhoto(string id, IFormFile AnhdaidienFile)
        {
            var nhanvienToUpdate = await _context.Nhanviens.FindAsync(id);
            if (nhanvienToUpdate == null) return NotFound();

            if (AnhdaidienFile != null && AnhdaidienFile.Length > 0)
            {
                // 1. Xoá ảnh cũ (nếu có)
                string uploadsFolder = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Nhanviens");
                if (!string.IsNullOrEmpty(nhanvienToUpdate.AnhNhanVien))
                {
                    string oldFilePath = Path.Combine(uploadsFolder, nhanvienToUpdate.AnhNhanVien);
                    if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                }

                // 2. Lưu ảnh mới
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + AnhdaidienFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AnhdaidienFile.CopyToAsync(stream);
                }

                // 3. Cập nhật CSDL
                nhanvienToUpdate.AnhNhanVien = uniqueFileName;
                await _context.SaveChangesAsync();
            }

            // Quay lại trang chi tiết của nhân viên đó
            return RedirectToAction(nameof(Details), new { id = nhanvienToUpdate.Manv});
        }
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        // GET: NhanVien/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens
                .Include(n => n.MabophanNavigation)
                .Include(n => n.TenchucvuNavigation)
                .Include(n => n.Nhanvienlamcas)
                    .ThenInclude(nl => nl.MacalamviecNavigation)
                .FirstOrDefaultAsync(m => m.Manv == id);
            if (nhanvien == null)
            {
                return NotFound();
            }

            return View(nhanvien);
        }
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhanvien = await _context.Nhanviens
                .Include(nv => nv.Nhanvienlamcas)
                .FirstOrDefaultAsync(m => m.Manv == id);

            if (nhanvien == null) { 
                return NotFound();
            }
            
            
            if (!string.IsNullOrEmpty(nhanvien.AnhNhanVien))
            {
                try
                {
                    var filePath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Nhanviens", nhanvien.AnhNhanVien);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                catch
                {
                    
                }
            }

            if (nhanvien.Nhanvienlamcas != null && nhanvien.Nhanvienlamcas.Any())
            {
                _context.Nhanvienlamcas.RemoveRange(nhanvien.Nhanvienlamcas);
            }

            // Xoá bản ghi nhân viên
            _context.Nhanviens.Remove(nhanvien);


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanvienExists(string id)
        {
            return _context.Nhanviens.Where(e => e.Manv == id).Count() > 0;
        }

        public IActionResult GetImage(string id)
        {
            var nhanvien = _context.Nhanviens.Find(id);
                

            if (nhanvien == null || string.IsNullOrEmpty(nhanvien.AnhNhanVien))
                return NotFound();

            // Xác định đường dẫn vật lý tới file
            var folderPath = Path.Combine(_env.ContentRootPath, "App_Data", "Uploads", "Nhanviens");
            var filePath = Path.Combine(folderPath, nhanvien.AnhNhanVien);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            // Xác định content-type theo đuôi file
            string contentType;
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, contentType);
        }
    }
}
