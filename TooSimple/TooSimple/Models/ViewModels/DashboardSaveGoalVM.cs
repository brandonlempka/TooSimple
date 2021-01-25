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
        public decimal GoalAmount { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal CurrentBalance { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime DesiredCompletionDate { get; set; }
    }
}
