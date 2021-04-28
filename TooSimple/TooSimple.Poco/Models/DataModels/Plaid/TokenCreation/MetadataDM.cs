using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.DataModels.Plaid.TokenCreation
{
    public class MetadataDM
    {
        public string[] initial_products { get; set; }
        public string[] country_codes { get; set; }
        public string language { get; set; }
        public AccountFiltersDM account_filters { get; set; }
        public string client_name { get; set; }
    }
}
