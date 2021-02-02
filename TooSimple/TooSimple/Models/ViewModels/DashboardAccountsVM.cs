using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ViewModels
{
    public class DashboardAccountsVM
    {
        public IEnumerable<DashboardAccountsListVM> Accounts { get; set; }
        public string LinkToken { get; set; }
    }
}
