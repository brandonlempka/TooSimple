using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ResponseModels.Plaid
{
    public class PlaidTransactionRequestRM
    {
        public List<PlaidAccountRM> accounts { get; set; }
        public List<PlaidTransactionRM> transactions { get; set; }
        public PlaidItemRM item { get; set; }
        public int total_transactions { get; set; }
        public string request_id { get; set; }
    }
}
