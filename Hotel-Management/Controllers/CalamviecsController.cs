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
    public class CalamviecsController : Controller
    {
        private readonly AppDbContext _context;

        public CalamviecsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Calamviecs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Calamviecs.ToListAsync());
        }

        // GET: Calamviecs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calamviec = await _context.Calamviecs
                .FirstOrDefaultAsync(m => m.Macalamviec == id);
            if (calamviec == null)
            {
                return NotFound();
            }

            return View(calamviec);
        }

        // GET: Calamviecs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Calamviecs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Macalamviec,Thoigianbatdau,Thoigianketthuc")] Calamviec calamviec)
        {
            if (ModelState.IsValid)
            {
                _context.Add(calamviec);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(calamviec);
        }

        // GET: Calamviecs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calamviec = await _context.Calamviecs.FindAsync(id);
            if (calamviec == null)
            {
                return NotFound();
            }
            return View(calamviec);
        }

        // POST: Calamviecs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Macalamviec,Thoigianbatdau,Thoigianketthuc")] Calamviec calamviec)
        {
            if (id != calamviec.Macalamviec)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(calamviec);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalamviecExists(calamviec.Macalamviec))
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
            return View(calamviec);
        }

        // GET: Calamviecs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calamviec = await _context.Calamviecs
                .FirstOrDefaultAsync(m => m.Macalamviec == id);
            if (calamviec == null)
            {
                return NotFound();
            }

            return View(calamviec);
        }

        // POST: Calamviecs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var calamviec = await _context.Calamviecs.FindAsync(id);
            if (calamviec != null)
            {
                _context.Calamviecs.Remove(calamviec);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CalamviecExists(string id)
        {
            return _context.Calamviecs.Any(e => e.Macalamviec == id);
        }
    }
}
