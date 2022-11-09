using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JyskCashpoint.Models
{
    public class Banknote
    {
        public int Id
        {
            get;
            set;
        }

        public int Value
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }
    }
}
