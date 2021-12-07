using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ViewModels
{
    public class DashboardGoalListVM
    {
        public IEnumerable<DashboardGoalVM> Goals { get; set; }
        public string NextContributionTotal { get; set; }
        public string NextContributionDate { get; set; }
    }
}
