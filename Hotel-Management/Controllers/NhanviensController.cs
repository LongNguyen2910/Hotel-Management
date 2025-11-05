using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hotel_Management.Controllers
{
    public class NhanviensController : Controller
    {
        private readonly AppDbContext _context;

        public NhanviensController(AppDbContext context)
        {
            _context = context;
        }

        // GET: NhanVien
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Nhanviens
                .Include(n => n.MabophanNavigation)
                .Include(n => n.TenchucvuNavigation)
                .Include(n=> n.Nhanvienlamcas)
                    .ThenInclude(nl => nl.MacalamviecNavigation);
            ViewBag.Trangthai = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Nghỉ việc", Value = "0" },
                new SelectListItem { Text = "Đang làm", Value = "1" }
            };
            return View(await modelContext.ToListAsync());
        }

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
        public IActionResult Create()
        {
            ViewData["Mabophan"] = new SelectList(_context.Bophans, "Mabophan", "Tenbophan");
            ViewData["Tenchucvu"] = new SelectList(_context.Chucvus, "Tenchucvu", "Tenchucvu");
            ViewData["Calamviec"] = new SelectList(_context.Calamviecs,"Macalamviec","Macalamviec");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Manv,Hoten,Gioitinh,Ngaysinh,Sodienthoai,Cccd,Ngayvaolam,Trangthai,Mabophan,Tenchucvu,Nhanvienlamcas")] 
            Nhanvien nhanvien, string SelectedCalamviec)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanvien);
                if (!string.IsNullOrEmpty(SelectedCalamviec))
                {
                    
                    var nhanvienlamca = new Nhanvienlamca
                    {
                        Manv = nhanvien.Manv, 
                        Macalamviec = SelectedCalamviec 
                    };
                    _context.Nhanvienlamcas.Add(nhanvienlamca); 
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Mabophan"] = new SelectList(_context.Bophans, "Mabophan", "Mabophan", nhanvien.Mabophan);
            ViewData["Tenchucvu"] = new SelectList(_context.Chucvus, "Tenchucvu", "Tenchucvu", nhanvien.Tenchucvu);
            ViewData["Calamviec"] = new SelectList(
               _context.Calamviecs,
               "Macalamviec",
               "Macalamviec",
               nhanvien.Nhanvienlamcas?.FirstOrDefault());
            ViewBag.Trangthai = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Nghỉ việc", Value = "0" },
                new SelectListItem { Text = "Đang làm", Value = "1" }
            };
            return View(nhanvien);
        }

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
            ViewData["Mabophan"] = new SelectList(_context.Bophans, "Mabophan", "Mabophan", nhanvien.Mabophan);
            ViewData["Tenchucvu"] = new SelectList(_context.Chucvus, "Tenchucvu", "Tenchucvu", nhanvien.Tenchucvu);

            string? currentShift = nhanvien.Nhanvienlamcas?.FirstOrDefault()?.Macalamviec;

            ViewData["Calamviec"] = new SelectList(_context.Calamviecs, "Macalamviec", "Macalamviec");
            ViewData["Calamviec"] = new SelectList(
               _context.Calamviecs,
               "Macalamviec",
               "Macalamviec",
               currentShift);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, 
            [Bind("Manv,Hoten,Gioitinh,Ngaysinh,Sodienthoai,Cccd,Ngayvaolam,Trangthai,Mabophan,Tenchucvu,Nhanvienlamcas")] 
            Nhanvien nhanvien, string SelectedCalamviec)
        {
            if (id != nhanvien.Manv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm ca làm việc hiện tại của nhân viên
                    var existingAssignment = await _context.Nhanvienlamcas
                        .FirstOrDefaultAsync(nlc => nlc.Manv == nhanvien.Manv);

                    // Xử lý ca làm việc được chọn
                    if (!string.IsNullOrEmpty(SelectedCalamviec))
                    {
                        if (existingAssignment != null)
                        {
                            // Nếu đã có -> Cập nhật
                            existingAssignment.Macalamviec = SelectedCalamviec;
                        }
                        else
                        {
                            // Nếu chưa có -> Tạo mới
                            var newAssignment = new Nhanvienlamca
                            {
                                Manv = nhanvien.Manv,
                                Macalamviec = SelectedCalamviec
                            };
                            _context.Nhanvienlamcas.Add(newAssignment);
                        }
                    }
                    else if (existingAssignment != null)
                    {
                        // Nếu người dùng bỏ chọn (chọn "Không chọn ca") -> Xóa
                        _context.Nhanvienlamcas.Remove(existingAssignment);
                    }
                    _context.Update(nhanvien);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["Mabophan"] = new SelectList(_context.Bophans, "Mabophan", "Mabophan", nhanvien.Mabophan);
            ViewData["Tenchucvu"] = new SelectList(_context.Chucvus, "Tenchucvu", "Tenchucvu", nhanvien.Tenchucvu);
            ViewData["Calamviec"] = new SelectList(_context.Calamviecs, "Macalamviec", "Macalamviec");
            ViewData["Calamviec"] = new SelectList(
               _context.Calamviecs,
               "Macalamviec",
               "Macalamviec",
               nhanvien.Nhanvienlamcas?.FirstOrDefault());
            ViewBag.Trangthai = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Nghỉ việc", Value = "0" },
                new SelectListItem { Text = "Đang làm", Value = "1" }
            };
            return View(nhanvien);
        }

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

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhanvien = await _context.Nhanviens.FindAsync(id);
            var nhanvienlamcas = _context.Nhanvienlamcas.Where(nlc => nlc.Manv == id); 
            if (nhanvien != null)
            {
                _context.Nhanviens.Remove(nhanvien);
                _context.Nhanvienlamcas.RemoveRange(nhanvienlamcas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanvienExists(string id)
        {
            return _context.Nhanviens.Any(e => e.Manv == id);
        }
    }
}
