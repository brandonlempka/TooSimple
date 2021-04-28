using System.Threading.Tasks;
using TooSimple.Models.ActionModels;
using TooSimple.Poco.Models.DataModels;
using TooSimple.Poco.Models.RequestModels;
using TooSimple.Poco.Models.ResponseModels;

namespace TooSimple.DataAccessors.TooSimple
{
    public interface IBudgetingDataAccessor
    {
        Task<GoalListDM> GetGoalListDMAsync(string userId);
        Task<GoalDM> GetGoalDMAsync(string goalId);
        Task<StatusRM>SaveGoalAsync(GoalDM actionModel);
        Task<StatusRM> DeleteGoalAsync(string goalId);
        Task<FundingScheduleListDM> GetFundingScheduleListDMAsync(string userId);
        Task<FundingScheduleDM> GetFundingScheduleDMAsync(string scheduleId);
        Task<StatusRM> SaveScheduleAsync(DashboardSaveFundingScheduleAM actionModel);
        Task<StatusRM> DeleteScheduleAsync(string scheduleId);
        Task<StatusRM> SaveMoveMoneyAsync(MoveMoneyRequestModel requestModel);
        Task<decimal?> CalculateUserAccountBalance(AccountListDM accountsDM, string userAccountId);
        Task<FundingHistoryListDM> GetFundingHistoryListDMAsync(string accountId);
    }
}