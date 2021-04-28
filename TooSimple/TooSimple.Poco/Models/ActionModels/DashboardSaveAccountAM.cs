using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ActionModels
{
    public class DashboardSaveAccountAM
    {
        public string AccountId { get; set; }
        public string NickName { get; set; }
        public bool UseForBudgeting { get; set; }
    }
}
