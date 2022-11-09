using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace JyskCashpoint.Models
{
    public class Cash
    {
        public Cash()
        {

        }

        public int Id
        {
            get;
            set;
        }

        [DefaultValue(7654)]
        public decimal Balance
        {
            get;
            set;
        }

        public string ApplicationUserId
        {
            get;
            set;
        }

        public virtual ApplicationUser ApplicationUser
        {
            get;
            set;
        }
    }
}
