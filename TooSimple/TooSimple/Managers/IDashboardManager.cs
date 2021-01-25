using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.Models.ActionModels;
using TooSimple.Models.DataModels.Plaid;
using TooSimple.Models.ResponseModels;
using TooSimple.Models.ResponseModels.Plaid;
using TooSimple.Models.ViewModels;

namespace TooSimple.Managers
{
    public interface IDashboardManager
    {
        Task<DashboardVM> GetDashboardVMAsync(ClaimsPrincipal currentUser);
        Task<StatusRM> PublicTokenExchangeAsync(PublicTokenRM dataModel, ClaimsPrincipal currentUser);
        Task<DashboardAccountsVM> GetDashboardAccountsVMAsync(ClaimsPrincipal currentUser);
        Task<DashboardEditAccountVM> GetIndividualAccountVMAsync(string Id, ClaimsPrincipal currentUser);
        Task<StatusRM> UpdateAccountAsync(DashboardEditAccountAM actionModel);
        Task<StatusRM> DeleteAccountAsync(string accountId);
    }
}
