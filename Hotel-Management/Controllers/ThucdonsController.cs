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
    public class ThucdonsController : Controller
    {
        private readonly AppDbContext _context;

        public ThucdonsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Thucdons
        public async Task<IActionResult> Index()
        {
            var thucdons = _context.Thucdons.Include(t => t.Mamons);
            return View(await thucdons.ToListAsync());
        }

        // GET: Thucdons/Create
        public IActionResult Create()
        {
            ViewBag.AllMons =  _context.Mons.ToList();
            return View();
        }

        // POST: Thucdons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mathucdon,Ngayapdung,Ngaytao")] 
            Thucdon thucdon, string[] selectedMon)
        {
            if (ModelState.IsValid)
            {
                if (selectedMon != null)
                {
                    foreach (var monId in selectedMon)
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
            ViewData["Dsmon"] = new MultiSelectList(_context.Mons, "Mamon", "Tenmon", selectedMon);
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
          
            ViewBag.AllMons = _context.Mons.ToList();
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
            ViewBag.AllMons = _context.Mons.ToList();
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
            var thucdon = await _context.Thucdons.FindAsync(id,date);
            if (thucdon != null)
            {
                _context.Thucdons.Remove(thucdon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThucdonExists(int id, DateTime date)
        {
            return _context.Thucdons.Any(e => e.Mathucdon == id && e.Ngayapdung == date);
        }
    }
}
