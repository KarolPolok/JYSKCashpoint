using JyskCashpoint.Data;
using JyskCashpoint.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JyskCashpoint.Services
{
    public class BanknoteService : IBanknoteService
    {
        private readonly ApplicationDbContext _context;

        public BanknoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public dynamic ViewBag
        {
            get;
            set;
        }

        public async Task<WithdrawStatus> Withdraw(decimal amount, string userId)
        {
            //return View("Index", await _context.Product.Where(j => j.Name.Contains(searchPhrase)).ToListAsync());

            //Banknotes in Cashpoint
            List<Banknote> banknoteStateInDb = _context.Banknote.ToList();
            //Sum of cash in Cashpoint
            int sum = banknoteStateInDb.Sum(item => item.Quantity * item.Value);
            //List for view
            List<BanknotesHelper> banknotesList = new List<BanknotesHelper>();
            //Cash object for manipulation
            Cash cash = await _context.Cash.Where(j => j.ApplicationUserId == userId).FirstAsync();
            //Check if Cashpoint has enough banknotes
            bool properAmountOfBanknotes = CheckProperBanknotesAmount(ref banknoteStateInDb, amount);
            //Check if Cashpoint is able to withdraw that much money
            if (sum >= amount && (amount % 10 == 0) && properAmountOfBanknotes && cash.Balance >= amount)
            {
                decimal checkAmount = amount;
                decimal banknotesCount = 0;

                foreach (Banknote banknote in banknoteStateInDb)
                {
                    ProcessBanknotes(banknote, ref checkAmount, ref banknotesCount, ref banknotesList);
                }
                cash.Balance -= amount;
                _context.Update(cash);
                await _context.SaveChangesAsync();
                ViewBag.Banknotes = banknotesList;
                ViewBag.Ballance = cash.Balance;
                ViewBag.AllowedToWithdraw = true;
                return WithdrawStatus.Succes;
            }
            else
            {
                ViewBag.AllowedToWithdraw = false;
                ViewBag.Ballance = cash.Balance;
                if (amount % 10 != 0)
                {
                    return WithdrawStatus.CoinWarning;
                }
                else if (!properAmountOfBanknotes)
                {
                    return WithdrawStatus.NotEnoughBanknotes;
                }
                else if (cash.Balance < amount)
                {
                    return WithdrawStatus.BalanceTooLow;
                }
                return WithdrawStatus.Succes;
            }
        }

        public void ProcessBanknotes(Banknote banknoteObject, ref decimal checkAmount, ref decimal banknotesCount, ref List<BanknotesHelper> banknotesList)
        {
            if (banknoteObject.Quantity >= Math.Truncate(checkAmount / banknoteObject.Value))
            {
                banknotesCount = Math.Truncate(checkAmount / banknoteObject.Value);
                banknoteObject.Quantity -= (int)banknotesCount;
                _context.Update(banknoteObject);
                _context.SaveChangesAsync();

            }
            else
            {
                banknotesCount = banknoteObject.Quantity;
            }
            checkAmount = checkAmount - banknoteObject.Value * banknotesCount;
            banknotesList.Add(new BanknotesHelper() { BanknoteValue = banknoteObject.Value, BanknoteCount = (int)banknotesCount });
        }

        public bool CheckProperBanknotesAmount(ref List<Banknote> banknotes, decimal amount)
        {
            bool properAmount = false;
            banknotes = banknotes.OrderByDescending(b => b.Value).ToList();
            int checkValue = 0;
            foreach (Banknote banknote in banknotes)
            {
                if (banknote.Quantity > 0)
                {
                    for (int i = 1; i <= banknote.Quantity; i++)
                    {
                        if (checkValue == amount)
                        {
                            return true;
                        }
                        if ((checkValue + banknote.Value) > amount)
                        {
                            continue;
                        }
                        if ((checkValue + banknote.Value) <= amount)
                        {
                            checkValue += banknote.Value;
                        }
                    }
                }

            }
            if (checkValue != amount)
            {

                if (banknotes.Count > 0)
                {
                    banknotes.RemoveAt(0);
                    return CheckProperBanknotesAmount(ref banknotes, amount);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
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

    public enum WithdrawStatus
    {
        Succes,
        CoinWarning,
        NotEnoughBanknotes,
        BalanceTooLow
    }
}



