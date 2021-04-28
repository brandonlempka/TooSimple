using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ViewModels
{
    public class TransactionTableVM
    {
        public IEnumerable<TransactionListVM> Transactions { get; set; }
        public PagerVM PagerVM { get; set; }
    }
}
