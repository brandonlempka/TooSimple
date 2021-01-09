using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ResponseModels.Plaid
{
    public class CreateLinkTokenRM
    {
        public string Expiration { get; set; }
        public string LinkToken { get; set; }
        public string RequestId { get; set; }

    }
}
