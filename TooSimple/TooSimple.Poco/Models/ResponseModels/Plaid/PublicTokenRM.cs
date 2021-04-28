using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Poco.Models.DataModels.Plaid.TokenExchange;

namespace TooSimple.Poco.Models.ResponseModels.Plaid
{
    public class PublicTokenRM
    {
        public string public_token { get; set; }
        public IEnumerable<TEAccountsDM> accounts { get; set; }
        public TEInstitutionsDM institution { get; set; }
        public string link_session_id { get; set; }
    }
}
