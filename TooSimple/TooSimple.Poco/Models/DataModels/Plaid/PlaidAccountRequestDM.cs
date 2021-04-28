using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.DataModels.Plaid
{
    public class PlaidAccountRequestDM
    {
        public string client_id { get; set; }
        public string secret { get; set; }
        public string access_token { get; set; }
        public PlaidAccountOptionsDM options { get; set; }
    }
}
