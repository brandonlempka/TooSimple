﻿using System.Threading.Tasks;
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
        Task<FundingScheduleListDM> GetFundingScheduleListDMAsync(string userId);
        Task<FundingScheduleDM> GetFundingScheduleDMAsync(string scheduleId);
        Task<StatusRM> SaveScheduleAsync(DashboardSaveFundingScheduleAM actionModel);
    }
}