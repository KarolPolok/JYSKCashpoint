using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JyskCashpoint.Data;
using JyskCashpoint.Models;

namespace JyskCashpoint.Controllers
{
    public class BanknotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BanknotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Banknotes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Banknote.ToListAsync());
        }

        // GET: Banknotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banknote = await _context.Banknote
                .SingleOrDefaultAsync(m => m.Id == id);
            if (banknote == null)
            {
                return NotFound();
            }

            return View(banknote);
        }

        // GET: Banknotes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Banknotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Value,Quantity")] Banknote banknote)
        {
            if (ModelState.IsValid)
            {
                _context.Add(banknote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(banknote);
        }

        // GET: Banknotes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banknote = await _context.Banknote.SingleOrDefaultAsync(m => m.Id == id);
            if (banknote == null)
            {
                return NotFound();
            }
            return View(banknote);
        }

        // POST: Banknotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Value,Quantity")] Banknote banknote)
        {
            if (id != banknote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(banknote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BanknoteExists(banknote.Id))
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
            return View(banknote);
        }

        // GET: Banknotes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banknote = await _context.Banknote
                .SingleOrDefaultAsync(m => m.Id == id);
            if (banknote == null)
            {
                return NotFound();
            }

            return View(banknote);
        }

        // POST: Banknotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banknote = await _context.Banknote.SingleOrDefaultAsync(m => m.Id == id);
            _context.Banknote.Remove(banknote);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BanknoteExists(int id)
        {
            return _context.Banknote.Any(e => e.Id == id);
        }
    }
}
