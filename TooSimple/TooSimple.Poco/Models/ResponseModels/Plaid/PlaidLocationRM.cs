using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ResponseModels.Plaid
{
    public class PlaidLocationRM
    {
        public string address { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
        public double? lat { get; set; }
        public double? lon { get; set; }
        public string store_number { get; set; }
    }
}
