using Hotel_Management.Helpers;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Management.Controllers
{
    public class ChucvusController : Controller
    {
        private readonly AppDbContext _context;

        public ChucvusController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Policy = "CanViewData")]
        // GET: Chucvus
        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 10;
            var paginatedList = await PaginatedList<Chucvu>.CreateAsync(_context.Chucvus.AsNoTracking(), pageNumber ?? 1, pageSize);
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                return PartialView("ChucvuTable", paginatedList);
            }

            return View(paginatedList);
        }


        // GET: Chucvus/Create
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Chucvus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Tenchucvu,Luongcoban")] Chucvu chucvu)
        {
            if (ChucvuExists(chucvu.Tenchucvu) )
            {
                ModelState.AddModelError("Tenchucvu", "Chức vụ đã tồn tại.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(chucvu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chucvu);
        }

        // GET: Chucvus/Edit/5
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chucvu = await _context.Chucvus.FindAsync(id);
            if (chucvu == null)
            {
                return NotFound();
            }
            return View(chucvu);
        }

        // POST: Chucvus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Tenchucvu,Luongcoban")] Chucvu chucvu)
        {
            if (id != chucvu.Tenchucvu)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chucvu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChucvuExists(chucvu.Tenchucvu))
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
            return View(chucvu);
        }

        // GET: Chucvus/Delete/5
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chucvu = await _context.Chucvus
                .FirstOrDefaultAsync(m => m.Tenchucvu == id);
            if (chucvu == null)
            {
                return NotFound();
            }

            return View(chucvu);
        }

        // POST: Chucvus/Delete/5
        [Authorize(Roles = "Admin, Quản lý nhân sự, Quản lý khách sạn")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var chucvu = await _context.Chucvus
                .Include(cv => cv.Nhanviens)
                .FirstOrDefaultAsync(nv => nv.Tenchucvu == id);
            if (chucvu != null)
            {
                chucvu.Nhanviens.Clear(); 
                _context.Chucvus.Remove(chucvu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChucvuExists(string id)
        {
            return _context.Chucvus.Where(e => e.Tenchucvu == id).Count() > 0;
        }
    }
}
