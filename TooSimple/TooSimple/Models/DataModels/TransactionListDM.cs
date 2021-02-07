using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels
{
    public class TransactionListDM
    {
        public IEnumerable<TransactionDM> Transactions { get; set; }
    }
}
