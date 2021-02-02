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
        public decimal? GoalAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime? DesiredCompletionDate { get; set; }
        public bool ExpenseFlag { get; set; }
        public decimal? AmountNeededEachTimeFrame { get; set; }
        public DateTime? FirstCompletionDate { get; set; }
        public int? RecurrenceTimeFrame { get; set; }

    }
}
