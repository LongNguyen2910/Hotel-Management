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
    public class ThucdonsController : Controller
    {
        private readonly AppDbContext _context;

        public ThucdonsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Thucdons
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var thucdons = _context.Thucdons.Include(t => t.Mamons);

            int pageSize = 10;

            return View(await PaginatedList<Thucdon>.CreateAsync(thucdons.AsNoTracking(),pageNumber ?? 1,pageSize));
        }

        // GET: Thucdons/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.AllMons = await _context.Mons.ToListAsync();
            return View();
        }

        // POST: Thucdons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mathucdon,Ngayapdung,Ngaytao")] 
            Thucdon thucdon, string[] selectedMons)
        {
            if (_context.Thucdons.Where(t => t.Mathucdon == thucdon.Mathucdon).Count() > 0)
            {
                ModelState.AddModelError("Mathucdon", "Mã thực đơn này đã tồn tại. Vui lòng chọn mã khác.");
            }

            if (ModelState.IsValid)
            {
                if (selectedMons != null)
                {
                    foreach (var monId in selectedMons)
                    {
                        // Tìm món ăn trong DB
                        var monToAdd = await _context.Mons.FindAsync(monId);
                        if (monToAdd != null)
                        {
                            // Thêm vào collection của thực đơn
                            thucdon.Mamons.Add(monToAdd);
                        }
                    }
                }
                _context.Add(thucdon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AllMons = await _context.Mons.ToListAsync();
            return View(thucdon);
        }

        // GET: Thucdons/Edit/5
        public async Task<IActionResult> Edit(int? id, DateTime? date)
        {
            if (id == null || date == null)
            {
                return NotFound();
            }

            var thucdon = await _context.Thucdons
                .Include(t => t.Mamons)
                .FirstOrDefaultAsync(m => m.Mathucdon == id && m.Ngayapdung == date);
            if (thucdon == null)
            {
                return NotFound();
            }
          
            ViewBag.AllMons = await _context.Mons.ToListAsync();
            ViewBag.SelectedMonIds = thucdon.Mamons.Select(m => m.Mamon).ToHashSet();
            return View(thucdon);
        }

        // POST: Thucdons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DateTime? date,[Bind("Mathucdon,Ngayapdung,Ngaytao")] 
            Thucdon thucdon, string[] selectedMons)
        {
            if (date == null)
            {
                return NotFound();
            }

            var thucdonToUpdate = await _context.Thucdons
                .Include(t => t.Mamons)
                .FirstOrDefaultAsync(m => m.Mathucdon == id && m.Ngayapdung == date);
            if (thucdonToUpdate == null)
            {
                return NotFound();
            }

            if (thucdonToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật các thuộc tính đơn giản (Ngày tạo)
                    thucdonToUpdate.Ngaytao = thucdon.Ngaytao;

                    // Cập nhật danh sách Món ăn (cách đơn giản nhất)
                    // 1. Xoá tất cả các món cũ
                    thucdonToUpdate.Mamons.Clear();

                    // 2. Thêm lại các món mới được chọn
                    if (selectedMons != null)
                    {
                        foreach (var monId in selectedMons)
                        {
                            var monToAdd = await _context.Mons.FindAsync(monId);
                            if (monToAdd != null)
                            {
                                thucdonToUpdate.Mamons.Add(monToAdd);
                            }
                        }
                    }
                    _context.Update(thucdonToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThucdonExists(thucdon.Mathucdon, thucdon.Ngayapdung))
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
            ViewBag.AllMons = await _context.Mons.ToListAsync();
            return View(thucdon);
        }

        // GET: Thucdons/Delete/5
        public async Task<IActionResult> Delete(int? id, DateTime? date)
        {
            if (id == null && date == null)
            {
                return NotFound();
            }

            var thucdon = await _context.Thucdons
                .Include(t => t.Mamons)
                .FirstOrDefaultAsync(m => m.Mathucdon == id && m.Ngayapdung == date);
            if (thucdon == null)
            {
                return NotFound();
            }

            return View(thucdon);
        }

        // POST: Thucdons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, DateTime date)
        {
            var thucdon = await _context.Thucdons
                .Include(td => td.Mamons)
                .FirstOrDefaultAsync(td => td.Mathucdon == id && td.Ngayapdung == date);
            if (thucdon != null)
            {
                thucdon.Mamons.Clear();
                _context.Thucdons.Remove(thucdon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThucdonExists(int? id, DateTime? date)
        {
            return _context.Thucdons.Where(e => e.Mathucdon == id && e.Ngayapdung == date).Count() > 0;
        }


    }
}
