using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JyskCashpoint.Services
{
    public interface IBanknoteService
    {
        dynamic ViewBag
        {
            get;
            set;
        }

        Task<WithdrawStatus> Withdraw(decimal amount, string userId);
    }
}
