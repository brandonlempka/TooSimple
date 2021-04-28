using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.DataModels.Plaid
{
    public class PlaidAccountOptionsDM
    {
        //[JsonProperty("account_ids")]
        public string[] account_ids { get; set; }

    }
}
