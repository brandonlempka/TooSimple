using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ViewModels
{
    public class DashboardGoalVM
    {
        public string GoalId { get; set; }
        public string UserAccountId { get; set; }
        public string GoalName { get; set; }
        public string GoalAmount { get; set; }
        public string CurrentBalance { get; set; }
        public DateTime? DesiredCompletionDate { get; set; }
        public string AutoSpendMerchantName { get; set; }
        public bool Paused { get; set; }
        public bool ExpenseFlag { get; set; }
        public int? RecurrenceTimeFrame { get; set; }
        public string ProgressPercent { get; set; }
    }
}
