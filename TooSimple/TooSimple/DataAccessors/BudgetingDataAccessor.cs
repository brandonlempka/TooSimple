using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Data;
using TooSimple.Models.ActionModels;
using TooSimple.Models.DataModels;
using TooSimple.Models.EFModels;
using TooSimple.Models.ResponseModels;

namespace TooSimple.DataAccessors
{
    public class BudgetingDataAccessor : IBudgetingDataAccessor
    {
        private ApplicationDbContext _db;

        public BudgetingDataAccessor(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GoalListDM> GetGoalListDMAsync(string userId)
        {
            var dataModel = new GoalListDM
            {
                Goals = from goal in _db.Goals
                        where goal.UserAccountId == userId
                        select new GoalDM
                        {
                            GoalAmount = goal.GoalAmount,
                            UserAccountId = goal.UserAccountId,
                            CurrentBalance = goal.CurrentBalance,
                            DesiredCompletionDate = goal.DesiredCompletionDate,
                            GoalId = goal.GoalId,
                            GoalName = goal.GoalName
                        }
            };

            return dataModel;

        }

        public async Task<GoalDM> GetGoalDMAsync(string goalId)
        {
            var data = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == goalId);

            if (data == null)
            {
                return new GoalDM();
            }

            return new GoalDM
            {
                GoalAmount = data.GoalAmount,
                CurrentBalance = data.CurrentBalance,
                DesiredCompletionDate = data.DesiredCompletionDate,
                GoalId = data.GoalId,
                GoalName = data.GoalName
            };
        }

        public async Task<StatusRM> SaveGoalAsync(DashboardSaveGoalAM actionModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(actionModel.GoalId))
                {
                    await _db.Goals.AddAsync(new Goal
                    {
                        GoalAmount = actionModel.GoalAmount,
                        CurrentBalance = 0,
                        DesiredCompletionDate = actionModel.DesiredCompletionDate,
                        GoalName = actionModel.GoalName,
                        UserAccountId = actionModel.UserAccountId
                    });

                    await _db.SaveChangesAsync();

                    return StatusRM.CreateSuccess(null, "Success");
                }

                var existingGoal = await _db.Goals.FirstOrDefaultAsync(x => x.GoalId == actionModel.GoalId);

                existingGoal.GoalName = actionModel.GoalName;
                existingGoal.GoalAmount = actionModel.GoalAmount;
                existingGoal.DesiredCompletionDate = actionModel.DesiredCompletionDate;
                existingGoal.CurrentBalance = actionModel.CurrentBalance;

                await _db.SaveChangesAsync();

                return StatusRM.CreateSuccess(null, "Success");
            }
            catch (Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }

        public async Task<FundingScheduleListDM> GetFundingScheduleListDMAsync(string userId)
        {
            var dataModel = new FundingScheduleListDM
            {
                FundingSchedules = from schedule in _db.FundingSchedules
                                   where schedule.UserAccountId == userId
                                   select new FundingScheduleDM
                                   {
                                       UserAccountId = schedule.UserAccountId,
                                       FirstContributionDate = schedule.FirstContributionDate,
                                       Frequency = schedule.Frequency,
                                       FundingScheduleId = schedule.FundingScheduleId,
                                       FundingScheduleName = schedule.FundingScheduleName
                                   }
            };

            return dataModel;
        }

        public async Task<FundingScheduleDM> GetFundingScheduleDMAsync(string scheduleId)
        {
            var data = await _db.FundingSchedules.FirstOrDefaultAsync(schedule => schedule.FundingScheduleId == scheduleId);

            if (data == null)
            {
                return new FundingScheduleDM();
            }

            return new FundingScheduleDM
            {
                UserAccountId = data.UserAccountId,
                FirstContributionDate = data.FirstContributionDate,
                Frequency = data.Frequency,
                FundingScheduleId = data.FundingScheduleId,
                FundingScheduleName = data.FundingScheduleName
            };
        }

        public async Task<StatusRM> SaveScheduleAsync(DashboardSaveFundingScheduleAM actionModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(actionModel.FundingScheduleId))
                {
                    await _db.FundingSchedules.AddAsync(new FundingSchedule
                    {
                        UserAccountId = actionModel.UserAccountId,
                        FirstContributionDate = actionModel.FirstContributionDate,
                        Frequency = actionModel.Frequency,
                        FundingScheduleName = actionModel.FundingScheduleName
                    });

                    await _db.SaveChangesAsync();
                    return StatusRM.CreateSuccess(null, "Successfully added funding schedule.");
                }

                var existingSchedule = await _db.FundingSchedules.FirstOrDefaultAsync(schedule => schedule.FundingScheduleId == actionModel.FundingScheduleId);

                existingSchedule.FundingScheduleName = actionModel.FundingScheduleName;
                existingSchedule.Frequency = actionModel.Frequency;
                existingSchedule.FirstContributionDate = actionModel.FirstContributionDate;

                await _db.SaveChangesAsync();
                return StatusRM.CreateSuccess(null, "Successfully saved funding schedule.");
            }
            catch (Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }
    }
}
