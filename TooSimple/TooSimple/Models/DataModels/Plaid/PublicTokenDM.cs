using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Models.DataModels.Plaid.TokenExchange;

namespace TooSimple.Models.DataModels.Plaid
{
    public class PublicTokenDM
    {
        public string public_token { get; set; }
        public IEnumerable<TEAccountsDM> accounts { get; set; }
        public TEInstitutionsDM institution { get; set; }
        public string link_session_id { get; set; }
    }
}
