using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels
{
    public class FundingScheduleDM
    {
        public string FundingScheduleId { get; set; }
        public string UserAccountId { get; set; }
        public string FundingScheduleName { get; set; }
        public int Frequency { get; set; }
        public DateTime FirstContributionDate { get; set; }
    }
}
