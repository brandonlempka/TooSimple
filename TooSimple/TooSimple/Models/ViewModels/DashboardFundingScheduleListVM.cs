using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ViewModels
{
    public class DashboardFundingScheduleListVM
    {
        public IEnumerable<DashboardFundingScheduleVM> FundingSchedules { get; set; }

    }
}
