using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.DataModels.Plaid.TokenCreation
{
    public class AccountFiltersDM
    {
        public DepositoryDM depository { get; set; }
        public CreditDM credit { get; set; }
    }
}
