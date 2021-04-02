using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels.Plaid
{
    public class PlaidAccountRequestRM
    {
        public IEnumerable<PlaidAccountRM> accounts { get; set; }
        public PlaidItemRM item { get; set; }
        public string error_code { get; set; }
        public string error_message { get; set; }
        public string request_id { get; set; }
    }
}
