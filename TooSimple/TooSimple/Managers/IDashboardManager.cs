using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.Models.ActionModels;
using TooSimple.Models.DataModels;
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
        Task<StatusRM> UpdateAccountAsync(DashboardSaveAccountAM actionModel);
        Task<DashboardGoalListVM> GetGoalsVMAsync(ClaimsPrincipal currentUser, bool isExpense = false);
        Task<StatusRM> DeleteAccountAsync(string accountId);
        Task<DashboardSaveGoalVM> GetSaveGoalVMAsync(string goalId, ClaimsPrincipal currentUser, bool isExpense);
        Task<StatusRM> UpdateGoalAsync(DashboardSaveGoalAM actionModel);
        Task<StatusRM> DeleteGoalAsync(string goalId);
        Task<DashboardEditTransactionVM> GetEditTransactionVMAsync(string transactionId, ClaimsPrincipal currentUser);
        Task<StatusRM> UpdateTransactionAsync(DashboardEditTransactionAM actionModel);
        Task<DashboardFundingScheduleListVM> GetDashboardFundingScheduleListVM(ClaimsPrincipal currentUser);
        Task<DashboardFundingScheduleVM> GetDashboardFundingScheduleVM(string scheduleId, ClaimsPrincipal currentUser);
        Task<StatusRM> UpdateFundingScheduleAsync(DashboardSaveFundingScheduleAM actionModel);
        Task<StatusRM> DeleteFundingScheduleAsync(string scheduleId);
        Task<DashboardMoveMoneyVM> GetMoveMoneyVMAsync(ClaimsPrincipal currentUser);
        Task<StatusRM> SaveMoveMoneyAsync(DashboardMoveMoneyAM actionModel, bool autoRefill = true);
        Task UpdateGoalFunding(string userId, DateTime todayDateTime);
        ContributionDM CalculateNextContribution(GoalDM goal, FundingScheduleDM schedule, DateTime todayDate);
    }
}
