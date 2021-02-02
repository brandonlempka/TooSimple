using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ViewModels
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
        public decimal CurrentBalance { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime? DesiredCompletionDate { get; set; }
        public List<SelectListItem> FundingScheduleOptions { get; set; }
        public string FundingScheduleId { get; set; }
        public bool ExpenseFlag { get; set; }
        public DateTime? FirstCompletionDate { get; set; }
        public decimal? AmountNeededEachTimeFrame { get; set; }
        public int? RecurrenceTimeFrame { get; set; }
        public List<SelectListItem> RecurrenceTimeFrameOptions { get; set; }
    }
}
