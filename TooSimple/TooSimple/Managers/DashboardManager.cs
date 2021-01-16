using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.Models.ViewModels;

namespace TooSimple.Managers
{
    public class DashboardManager : IDashboardManager
    {
        public async Task<DashboardVM> GetDashboardVMAsync(ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            return new DashboardVM();
        }
    }
}
