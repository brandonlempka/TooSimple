using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ViewModels
{
    public class DashboardFundingHistoryVM
    {
        public string FundingHistoryId { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransferDate { get; set; }
        public string Note { get; set; }
        public bool AutomatedTransfer { get; set; }
    }
}
