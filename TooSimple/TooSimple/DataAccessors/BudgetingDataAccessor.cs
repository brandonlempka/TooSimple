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
            var data = await _db.Goals.FindAsync(goalId);

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
    }
}
