using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Management.Models;

namespace Hotel_Management.Controllers
{
    public class NhanviensController : Controller
    {
        private readonly AppDbContext _context;

        public NhanviensController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Nhanviens
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Nhanviens.Include(n => n.MabophanNavigation).Include(n => n.TenchucvuNavigation);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Nhanviens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens
                .Include(n => n.MabophanNavigation)
                .Include(n => n.TenchucvuNavigation)
                .FirstOrDefaultAsync(m => m.Manv == id);
            if (nhanvien == null)
            {
                return NotFound();
            }

            return View(nhanvien);
        }

        // GET: Nhanviens/Create
        public IActionResult Create()
        {
            ViewData["Mabophan"] = new SelectList(_context.Bophans, "Mabophan", "Mabophan");
            ViewData["Tenchucvu"] = new SelectList(_context.Chucvus, "Tenchucvu", "Tenchucvu");
            return View();
        }

        // POST: Nhanviens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Manv,Hoten,Gioitinh,Ngaysinh,Sodienthoai,Cccd,Ngayvaolam,Trangthai,Mabophan,Tenchucvu")] Nhanvien nhanvien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanvien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Mabophan"] = new SelectList(_context.Bophans, "Mabophan", "Mabophan", nhanvien.Mabophan);
            ViewData["Tenchucvu"] = new SelectList(_context.Chucvus, "Tenchucvu", "Tenchucvu", nhanvien.Tenchucvu);
            return View(nhanvien);
        }

        // GET: Nhanviens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens.FindAsync(id);
            if (nhanvien == null)
            {
                return NotFound();
            }
            ViewData["Mabophan"] = new SelectList(_context.Bophans, "Mabophan", "Mabophan", nhanvien.Mabophan);
            ViewData["Tenchucvu"] = new SelectList(_context.Chucvus, "Tenchucvu", "Tenchucvu", nhanvien.Tenchucvu);
            return View(nhanvien);
        }

        // POST: Nhanviens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Manv,Hoten,Gioitinh,Ngaysinh,Sodienthoai,Cccd,Ngayvaolam,Trangthai,Mabophan,Tenchucvu")] Nhanvien nhanvien)
        {
            if (id != nhanvien.Manv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            return View(nhanvien);
        }

        // GET: Nhanviens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens
                .Include(n => n.MabophanNavigation)
                .Include(n => n.TenchucvuNavigation)
                .FirstOrDefaultAsync(m => m.Manv == id);
            if (nhanvien == null)
            {
                return NotFound();
            }

            return View(nhanvien);
        }

        // POST: Nhanviens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhanvien = await _context.Nhanviens.FindAsync(id);
            if (nhanvien != null)
            {
                _context.Nhanviens.Remove(nhanvien);
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
