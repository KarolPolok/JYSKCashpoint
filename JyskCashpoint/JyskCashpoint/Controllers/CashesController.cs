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

namespace JyskCashpoint.Controllers
{
    public class CashesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CashesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cashes
        public async Task<IActionResult> Index()
        {
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
            var userObject = _userManager.GetUserId(HttpContext.User);

            //Banknotes in Cashpoint
            List<Banknote> banknoteStateInDb = _context.Banknote.ToList();
            //Sum of cash in Cashpoint
            int sum = banknoteStateInDb.Sum(item => item.Quantity * item.Value);
            //List for view
            List<BanknotesHelper> banknotesList = new List<BanknotesHelper>();
            //Cash object for manipulation
            Cash cash = await _context.Cash.Where(j => j.ApplicationUserId == userObject).FirstAsync();
            //Check if Cashpoint has enough banknotes
            bool properAmountOfBanknotes = CheckProperBanknotesAmount(banknoteStateInDb, amount);
            //Check if Cashpoint is able to withdraw that much money
            if (sum >= amount && (amount % 10 == 0) && properAmountOfBanknotes)
            {
                decimal checkAmount = amount;
                decimal banknotesCount;
                var twoHundreds = banknoteStateInDb.Where(j => j.Value == 200).First();
                var oneHundred = banknoteStateInDb.Where(j => j.Value == 100).First();
                var fiftys = banknoteStateInDb.Where(j => j.Value == 50).First();
                var twentys = banknoteStateInDb.Where(j => j.Value == 20).First();
                var tens = banknoteStateInDb.Where(j => j.Value == 10).First();
                if (checkAmount / 200 >= 1)
                {
                    if (twoHundreds.Quantity >= Math.Truncate(checkAmount / 200))
                    {
                        banknotesCount = Math.Truncate(checkAmount / 200);
                        twoHundreds.Quantity -= (int)banknotesCount;
                        _context.Update(twoHundreds);
                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        banknotesCount = twoHundreds.Quantity;
                    }
                    checkAmount = checkAmount - 200 * banknotesCount;
                    banknotesList.Add(new BanknotesHelper() { BanknoteValue = 200, BanknoteCount = (int)banknotesCount });
                }
                if (checkAmount / 100 >= 1)
                {
                    if (oneHundred.Quantity >= Math.Truncate(checkAmount / 100))
                    {
                        banknotesCount = Math.Truncate(checkAmount / 100);
                        oneHundred.Quantity -= (int)banknotesCount;
                        _context.Update(oneHundred);
                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        banknotesCount = oneHundred.Quantity;
                    }
                    checkAmount = checkAmount - 100 * banknotesCount;
                    banknotesList.Add(new BanknotesHelper() { BanknoteValue = 100, BanknoteCount = (int)banknotesCount });
                }
                if (checkAmount / 50 >= 1)
                {
                    if (fiftys.Quantity >= Math.Truncate(checkAmount / 50))
                    {
                        banknotesCount = Math.Truncate(checkAmount / 50);
                        fiftys.Quantity -= (int)banknotesCount;
                        _context.Update(fiftys);
                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        banknotesCount = fiftys.Quantity;
                    }
                    checkAmount = checkAmount - 50 * banknotesCount;
                    banknotesList.Add(new BanknotesHelper() { BanknoteValue = 50, BanknoteCount = (int)banknotesCount });
                }
                if (checkAmount / 20 >= 1)
                {
                    if (twentys.Quantity >= Math.Truncate(checkAmount / 20))
                    {
                        banknotesCount = Math.Truncate(checkAmount / 20);
                        twentys.Quantity -= (int)banknotesCount;
                        _context.Update(twentys);
                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        banknotesCount = twentys.Quantity;
                    }
                    checkAmount = checkAmount - 20 * banknotesCount;
                    banknotesList.Add(new BanknotesHelper() { BanknoteValue = 20, BanknoteCount = (int)banknotesCount });
                }
                if (checkAmount / 10 >= 1)
                {
                    if (tens.Quantity >= Math.Truncate(checkAmount / 10))
                    {
                        banknotesCount = Math.Truncate(checkAmount / 10);
                        tens.Quantity -= (int)banknotesCount;
                        _context.Update(tens);
                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        banknotesCount = tens.Quantity;
                    }
                    checkAmount = checkAmount - 10 * banknotesCount;
                    banknotesList.Add(new BanknotesHelper() { BanknoteValue = 10, BanknoteCount = (int)banknotesCount });
                }
                cash.Balance -= amount;
                _context.Update(cash);
                await _context.SaveChangesAsync();
                ViewBag.Banknotes = banknotesList;
                ViewBag.Ballance = cash.Balance;
                ViewBag.AllowedToWithdraw = true;
                return View("WithdrawInfo");
            }
            else
            {
                ViewBag.AllowedToWithdraw = false;
                ViewBag.Ballance = cash.Balance;
                if(amount % 10 != 0)
                {
                    ViewBag.ErrorMessage = "Make sure you chose amount that does not require coins to withdraw like 14zł, 358zł or 122zł";
                }
                else if(!properAmountOfBanknotes)
                {
                    ViewBag.ErrorMessage = "Not enough banknotes in cashpoint, try another amount";
                }
                return View("WithdrawInfo");
            }

        }

        public bool CheckProperBanknotesAmount(List<Banknote> banknotes, decimal amount)
        {
            bool properAmount = false;
            banknotes = banknotes.OrderByDescending(b => b.Value).ToList();
            int checkValue = 0;
            foreach(Banknote banknote in banknotes)
            {
                if(banknote.Quantity > 0)
                {
                    for (int i = 1; i <= banknote.Quantity; i++)
                    {
                        if(checkValue == amount)
                        {
                            return true;
                        }
                        if((checkValue + banknote.Value) > amount)
                        {
                            continue;
                        }
                        if((checkValue + banknote.Value) <= amount)
                        {
                            checkValue += banknote.Value;
                        }
                    }
                }

            }
            if(checkValue != amount)
            {
                return false;
            }
            else
            {
                return true;
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

    public class BanknotesHelper
    {
        public BanknotesHelper()
        {

        }

        public int BanknoteValue
        {
            get;
            set;
        }
        public int BanknoteCount
        {
            get;
            set;
        }
    }
}
