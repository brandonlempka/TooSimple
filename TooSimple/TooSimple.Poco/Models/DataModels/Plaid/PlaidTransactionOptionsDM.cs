using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.DataModels.Plaid
{
    public class PlaidTransactionOptionsDM
    {
        public string[] account_ids { get; set; }
        public int count { get; set; }
        public int offset { get; set; }
    }
}
