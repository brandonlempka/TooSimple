using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels.Plaid.TokenExchange
{
    public class TEUpdateDM
    {
        public string client_id { get; set; }
        public string secret { get; set; }
        public string client_name { get; set; }
        public string[] country_codes { get; set; }
        public string language { get; set; }
        public string access_token { get; set; }
        public TEUserDM user { get; set; }
    }
}
