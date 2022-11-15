using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JyskCashpoint.Data;
using JyskCashpoint.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using JyskCashpoint.Services;

namespace JyskCashpoint.Controllers
{
    public class CashesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IBanknoteService _banknoteService;

        public CashesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBanknoteService banknoteService)
        {
            _context = context;
            _userManager = userManager;
            _banknoteService = banknoteService;
        }

        // GET: Cashes
        public async Task<IActionResult> Index()
        {
            var userObject = _userManager.GetUserId(HttpContext.User);
            if (userObject == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var applicationDbContext = _context.Cash.Include(c => c.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Cashes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cash = await _context.Cash
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cash == null)
            {
                return NotFound();
            }

            return View(cash);
        }

        // GET: Cashes/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Cashes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Balance,ApplicationUserId")] Cash cash)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cash);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", cash.ApplicationUserId);
            return View(cash);
        }

        // GET: Cashes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cash = await _context.Cash.SingleOrDefaultAsync(m => m.Id == id);
            if (cash == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", cash.ApplicationUserId);
            return View(cash);
        }

        // GET: Products/ShowSearchForm
        public async Task<IActionResult> ShowWithdrawForm()
        {
            var userObject = _userManager.GetUserId(HttpContext.User);
            if (userObject == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // GET: Products/ShowSearchResult
        public async Task<IActionResult> Withdraw(decimal amount)
        {
            //return View("Index", await _context.Product.Where(j => j.Name.Contains(searchPhrase)).ToListAsync());
            string userId = _userManager.GetUserId(HttpContext.User);

            _banknoteService.ViewBag = ViewBag;
            WithdrawStatus status = _banknoteService.Withdraw(amount, userId).Result;

            //Check if Cashpoint is able to withdraw that much money
            if (status == WithdrawStatus.Succes)
            {
                ViewBag.Banknotes = _banknoteService.ViewBag.Banknotes;
                ViewBag.Ballance = _banknoteService.ViewBag.Ballance;
                ViewBag.AllowedToWithdraw = true;
                return View("WithdrawInfo");
            }
            else
            {
                ViewBag.AllowedToWithdraw = false;
                ViewBag.Ballance = _banknoteService.ViewBag.Ballance;
                if (status == WithdrawStatus.CoinWarning)
                {
                    ViewBag.ErrorMessage = "Make sure you chose amount that does not require coins to withdraw like 14zł, 358zł or 122zł";
                }
                else if(status == WithdrawStatus.NotEnoughBanknotes)
                {
                    ViewBag.ErrorMessage = "Not enough banknotes in cashpoint, try another amount";
                }
                else if (status == WithdrawStatus.BalanceTooLow)
                {
                    ViewBag.ErrorMessage = "Your account balance is lower than amount to withdraw";
                }
                return View("WithdrawInfo");
            }

        }


        // POST: Cashes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Balance,ApplicationUserId")] Cash cash)
        {
            if (id != cash.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cash);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CashExists(cash.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", cash.ApplicationUserId);
            return View(cash);
        }

        // GET: Cashes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cash = await _context.Cash
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cash == null)
            {
                return NotFound();
            }

            return View(cash);
        }

        // POST: Cashes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cash = await _context.Cash.SingleOrDefaultAsync(m => m.Id == id);
            _context.Cash.Remove(cash);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CashExists(int id)
        {
            return _context.Cash.Any(e => e.Id == id);
        }
    }
}
