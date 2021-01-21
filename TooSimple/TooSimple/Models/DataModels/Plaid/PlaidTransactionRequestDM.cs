using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels.Plaid
{
    public class PlaidTransactionRequestDM
    {
        public string clientId { get; set; }
        public string secret { get; set; }
        public string accessToken { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public PlaidTransactionRequestOptionsDM options { get; set; }
    }
}
