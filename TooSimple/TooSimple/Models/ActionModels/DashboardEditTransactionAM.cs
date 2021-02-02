using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ActionModels
{
    public class DashboardEditTransactionAM
    {
        public string TransactionId { get; set; }
        public string AccountId { get; set; }
        public string SpendingFromId { get; set; }
        public string InternalCategory { get; set; }
        public string UserAccountId { get; set; }
    }
}
