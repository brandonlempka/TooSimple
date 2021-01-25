using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.EFModels
{
    public class Expense
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ExpenseId { get; set; }
        public string UserAccountId { get; set; }
        public string ExpenseName { get; set; }
        public long RecurrenceTimeFrame { get; set; }
        public decimal AmountNeededEachTimeFrame { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime FirstCompletionDate { get; set; }
        public string FundingScheduleId { get; set; }
    }
}
