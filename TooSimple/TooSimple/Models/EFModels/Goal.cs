using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.EFModels
{
    public class Goal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string GoalId { get; set; }
        public string GoalName { get; set; }
        public decimal GoalAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime DesiredCompletionDate { get; set; }
    }
}
