using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ResponseModels.Plaid
{
    public class PlaidItemRM
    {
        public IEnumerable<string> available_products { get; set; }
        public IEnumerable<string> billed_products { get; set; }
        public object consent_expiration_time { get; set; }
        public object error { get; set; }
        public string institution_id { get; set; }
        public string item_id { get; set; }
        public string webhook { get; set; }
    }
}
