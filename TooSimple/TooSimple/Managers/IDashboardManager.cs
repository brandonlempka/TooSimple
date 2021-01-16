using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.Models.ViewModels;

namespace TooSimple.Managers
{
    public interface IDashboardManager
    {
        Task<DashboardVM> GetDashboardVMAsync(ClaimsPrincipal currentUser);
    }
}
