using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.EFModels
{
    public class FundingSchedule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string FundingScheduleId { get; set; }
        public string FundingScheduleName { get; set; }
        public long Frequency { get; set; }
        public DateTime FirstContributionDate { get; set; }
    }
}
