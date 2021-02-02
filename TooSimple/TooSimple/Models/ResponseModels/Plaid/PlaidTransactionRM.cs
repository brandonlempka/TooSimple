using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ResponseModels.Plaid
{
    public class PlaidTransactionRM
    {
        public string account_id { get; set; }
        public decimal? amount { get; set; }
        public string iso_currency_code { get; set; }
        public object unofficial_currency_code { get; set; }
        public List<string> category { get; set; }
        public string category_id { get; set; }
        public string date { get; set; }
        public string authorized_date { get; set; }
#nullable enable
        public PlaidLocationRM? location { get; set; }
#nullable disable
        public string name { get; set; }
        public string merchant_name { get; set; }
#nullable enable
        public PlaidPaymentMetaRM? payment_meta { get; set; }
#nullable disable
        public string payment_channel { get; set; }
        public bool pending { get; set; }
        public string pending_transaction_id { get; set; }
        public string account_owner { get; set; }
        public string transaction_id { get; set; }
        public string transaction_code { get; set; }
        public string transaction_type { get; set; }
    }
}
