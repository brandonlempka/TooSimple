using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ResponseModels.Plaid
{
    public class TokenExchangeRM
    {
        public string Access_token { get; set; }
        public string Item_id { get; set; }
        public string Request_id { get; set; }
    }
}
