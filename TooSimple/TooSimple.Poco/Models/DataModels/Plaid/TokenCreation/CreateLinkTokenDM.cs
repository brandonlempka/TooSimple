using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.DataModels.Plaid.TokenCreation
{
    public class CreateLinkTokenDM
    {
        public string client_id { get; set; }
        public string secret { get; set; }
        public string client_name { get; set; }
        public string[] country_codes { get; set; }
        public string language { get; set; }
        public UserDM user { get; set; }
        public string[] products { get; set; }
        public AccountFiltersDM account_filters { get; set; }
    }
}
