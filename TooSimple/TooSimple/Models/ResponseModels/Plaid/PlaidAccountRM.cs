using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels.Plaid
{
    public class PlaidAccountRM
    {
        public string account_id { get; set; }
        public PlaidBalanceRM balances { get; set; }
        public string mask { get; set; }
        public string name { get; set; }
        public string official_name { get; set; }
        public string subtype { get; set; }
        public string type { get; set; }
    }
}
