using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ViewModels
{
    public class DashboardSaveGoalVM
    {
        public string GoalId { get; set; }
        public string UserAccountId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string GoalName { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal? GoalAmount { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime? DesiredCompletionDate { get; set; }
        public string AutoSpendMerchantName { get; set; }
        public bool Paused { get; set; }
        public bool AutoRefill { get; set; }
        public List<SelectListItem> FundingScheduleOptions { get; set; }
        public string FundingScheduleId { get; set; }
        public bool ExpenseFlag { get; set; }
        public int? RecurrenceTimeFrame { get; set; }
        public List<SelectListItem> RecurrenceTimeFrameOptions { get; set; }
        public DateTime CreationDate { get; set; }
        public List<DashboardFundingHistoryVM> FundingHistory { get; set; }
    }
}
