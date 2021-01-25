using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels
{
    public class AccountListDM
    {
        public IEnumerable<AccountDM> Accounts { get; set; }
    }
}
