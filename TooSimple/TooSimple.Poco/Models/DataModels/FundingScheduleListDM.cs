using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.DataModels
{
    public class FundingScheduleListDM
    {
        public IEnumerable<FundingScheduleDM> FundingSchedules { get; set; }
    }
}
