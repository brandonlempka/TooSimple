using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels.Plaid
{
    public class PlaidBalanceRM
    {
        public decimal? available { get; set; }
        public decimal? current { get; set; }
        public string iso_currency_code { get; set; }
        public int? limit { get; set; }
        public object unofficial_currency_code { get; set; }
    }
}
