using System.Threading.Tasks;
using TooSimple.Models.ActionModels;
using TooSimple.Models.DataModels;
using TooSimple.Models.ResponseModels;

namespace TooSimple.DataAccessors
{
    public interface IBudgetingDataAccessor
    {
        Task<GoalListDM> GetGoalListDMAsync(string userId);
        Task<GoalDM> GetGoalDMAsync(string goalId);
        Task<StatusRM>SaveGoalAsync(DashboardSaveGoalAM actionModel);
        Task<StatusRM> DeleteGoalAsync(string goalId);
        Task<FundingScheduleListDM> GetFundingScheduleListDMAsync(string userId);
        Task<FundingScheduleDM> GetFundingScheduleDMAsync(string scheduleId);
        Task<StatusRM> SaveScheduleAsync(DashboardSaveFundingScheduleAM actionModel);
        Task<StatusRM> DeleteScheduleAsync(string scheduleId);
        Task<StatusRM> SaveMoveMoneyAsync(DashboardMoveMoneyAM actionModel);
        Task<decimal?> CalculateUserAccountBalance(AccountListDM accountsDM, string userAccountId);
        Task<FundingHistoryListDM> GetFundingHistoryListDMAsync(string accountId);
    }
}