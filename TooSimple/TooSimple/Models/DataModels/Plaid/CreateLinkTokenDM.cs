using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels.Plaid
{
    public class CreateLinkTokenDM
    {
        public string client_id { get; set; }
        public string secret { get; set; }
        public string client_name { get; set; }
        public string[] country_codes { get; set; }
        public string language { get; set; }
        public CreateLinkTokenUserDM user { get; set; }
        public string[] products { get; set; }
    }
}
