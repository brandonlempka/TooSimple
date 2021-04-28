using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.DataModels.Plaid.TokenExchange
{
    public class TEAccountsDM
    {
        public string id { get; set; }
        public string name { get; set; }
        public string mask { get; set; }
        public string type { get; set; }
        public string subtype { get; set; }
        public string verification_status { get; set; }
    }
}
