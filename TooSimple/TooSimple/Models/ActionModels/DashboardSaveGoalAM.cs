﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ActionModels
{
    public class DashboardSaveGoalAM
    {
        public string GoalId { get; set; }
        public string UserAccountId { get; set; }
        public string GoalName { get; set; }
        public decimal? GoalAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime? DesiredCompletionDate { get; set; }
        public string FundingScheduleId { get; set; }
        public bool ExpenseFlag { get; set; }
        public DateTime? FirstCompletionDate { get; set; }
        public decimal? AmountNeededEachTimeFrame { get; set; }
        public int? RecurrenceTimeFrame { get; set; }

    }
}
