using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Models.DataModels.Plaid.TokenCreation;

namespace TooSimple.Models.DataModels.Plaid.TokenCreation
{
    public class AccountFiltersDM
    {
        public DepositoryDM depository { get; set; }
        public CreditDM credit { get; set; }
    }
}
