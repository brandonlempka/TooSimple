using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ResponseModels
{
    public class SaveTransactionRM
    {
        public string TransactionId { get; set; }
        public string ReferenceNumber { get; set; }
        public string AccountId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string SpendingFrom { get; set; }
        public string InternalCategory { get; set; }
        public string UserAccountId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
