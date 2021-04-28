using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ResponseModels.Plaid
{
    public class CreateLinkTokenRM
    {
        public string Expiration { get; set; }
        public string Link_Token { get; set; }
        public string Request_Id { get; set; }

    }
}
