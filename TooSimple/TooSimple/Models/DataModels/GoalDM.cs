using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels
{
    public class GoalDM
    {
        public string GoalId { get; set; }
        public string UserAccountId { get; set; }
        public string GoalName { get; set; }
        public decimal GoalAmount { get; set; }
        public decimal AmountContributed { get; set; }
        public decimal AmountSpent { get; set; }
        public bool AutoRefill { get; set; }
        public DateTime DesiredCompletionDate { get; set; }
        public string AutoSpendMerchantName { get; set; }
        public bool Paused { get; set; }
        public string FundingScheduleId { get; set; }
        public bool ExpenseFlag { get; set; }
        public int? RecurrenceTimeFrame { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime NextContributionDate { get; set; }
        public decimal NextContributionAmount { get; set; }
    }
}
