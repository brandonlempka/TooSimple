using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Data;
using TooSimple.Extensions;
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
                            GoalName = goal.GoalName,
                            FundingScheduleId = goal.FundingScheduleId,
                            ExpenseFlag = goal.ExpenseFlag,
                            RecurrenceTimeFrame = goal.RecurrenceTimeFrame,
                            CreationDate = goal.CreationDate,
                            Paused = goal.Paused
                        }
            };

            return dataModel;
        }

        public async Task<GoalDM> GetGoalDMAsync(string goalId)
        {
            var data = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == goalId);

            return new GoalDM
            {
                GoalAmount = data.GoalAmount,
                CurrentBalance = data.CurrentBalance,
                DesiredCompletionDate = data.DesiredCompletionDate,
                GoalId = data.GoalId,
                GoalName = data.GoalName,
                FundingScheduleId = data.FundingScheduleId,
                UserAccountId = data.UserAccountId,
                ExpenseFlag = data.ExpenseFlag,
                RecurrenceTimeFrame = data.RecurrenceTimeFrame,
                CreationDate = data.CreationDate,
                Paused = data.Paused
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
                        UserAccountId = actionModel.UserAccountId,
                        FundingScheduleId = actionModel.FundingScheduleId,
                        ExpenseFlag = actionModel.ExpenseFlag,
                        RecurrenceTimeFrame = actionModel.RecurrenceTimeFrame,
                        CreationDate = actionModel.CreationDate,
                        Paused = actionModel.Paused
                    });

                    await _db.SaveChangesAsync();

                    return StatusRM.CreateSuccess(null, "Success");
                }

                var existingGoal = await _db.Goals.FirstOrDefaultAsync(x => x.GoalId == actionModel.GoalId);

                existingGoal.GoalName = actionModel.GoalName;
                existingGoal.GoalAmount = actionModel.GoalAmount;
                existingGoal.DesiredCompletionDate = actionModel.DesiredCompletionDate;
                existingGoal.CurrentBalance = actionModel.CurrentBalance;
                existingGoal.FundingScheduleId = actionModel.FundingScheduleId;
                existingGoal.RecurrenceTimeFrame = actionModel.RecurrenceTimeFrame;
                existingGoal.ExpenseFlag = actionModel.ExpenseFlag;
                existingGoal.Paused = actionModel.Paused;

                await _db.SaveChangesAsync();

                return StatusRM.CreateSuccess(null, "Success");
            }
            catch (Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }

        public async Task<StatusRM> DeleteGoalAsync(string goalId)
        {
            try
            {
                var existingGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == goalId);
                _db.Goals.Remove(existingGoal);

                await _db.SaveChangesAsync();

                return StatusRM.CreateSuccess(null, "Successfully deleted.");
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

        public async Task<StatusRM> DeleteScheduleAsync(string scheduleId)
        {
            try
            {
                var existingSchedule = await _db.FundingSchedules.FirstOrDefaultAsync(schedule => schedule.FundingScheduleId == scheduleId);
                _db.FundingSchedules.Remove(existingSchedule);

                await _db.SaveChangesAsync();

                return StatusRM.CreateSuccess(null, "Successfully deleted funding schedule.");
            }
            catch (Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }

        public async Task<StatusRM> SaveMoveMoneyAsync(DashboardMoveMoneyAM actionModel)
        {
            try
            {
                if (actionModel.FromAccountId == "0")
                {
                    var existingGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == actionModel.ToAccountId);
                    existingGoal.CurrentBalance += actionModel.Amount;
                    await _db.SaveChangesAsync();
                }
                else if (actionModel.ToAccountId == "0")
                {
                    var existingGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == actionModel.FromAccountId);
                    existingGoal.CurrentBalance -= actionModel.Amount;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var fromGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == actionModel.FromAccountId);
                    var toGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == actionModel.ToAccountId);

                    fromGoal.CurrentBalance -= actionModel.Amount;
                    toGoal.CurrentBalance += actionModel.Amount;
                    await _db.SaveChangesAsync();
                }


                var historyEntry = new FundingHistory
                {
                    Amount = actionModel.Amount,
                    FromAccountId = actionModel.FromAccountId,
                    ToAccountId = actionModel.ToAccountId,
                    Note = actionModel.Note,
                    TransferDate = actionModel.TransferDate,
                    AutomatedTransfer = actionModel.AutomatedTransfer,
                };

                await _db.FundingHistories.AddAsync(historyEntry);

                await _db.SaveChangesAsync();
                return StatusRM.CreateSuccess(null, "Successfully moved money.");
            }
            catch (Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }

        public async Task<decimal?> CalculateUserAccountBalance(AccountListDM accountsDM, string userAccountId)
        {

            var goalDM = await GetGoalListDMAsync(userAccountId);
            var accountSum = accountsDM.Accounts.EmptyIfNull().Select(x => x.CurrentBalance).Sum();
            var goalsSum = goalDM.Goals.Select(x => x.CurrentBalance).Sum();
            decimal transactionsSum = 0;

            foreach (var account in accountsDM.Accounts)
            {
                transactionsSum += account.Transactions.Where(t => !string.IsNullOrWhiteSpace(t.SpendingFrom))
                    .Select(x => x.Amount).Sum();
            }

            accountSum -= goalsSum;
            accountSum += transactionsSum;

            return accountSum;
        }

        public async Task<FundingHistoryListDM> GetFundingHistoryListDMAsync(string accountId)
        {
            return new FundingHistoryListDM
            {
                FundingHistories = from history in _db.FundingHistories
                                   where history.ToAccountId == accountId || history.FromAccountId == accountId
                                   select new FundingHistoryDM
                                   {
                                       Amount = history.Amount,
                                       AutomatedTransfer = history.AutomatedTransfer,
                                       FromAccountId = history.FromAccountId,
                                       ToAccountId = history.ToAccountId,
                                       FundingHistoryId = history.FundingHistoryId,
                                       Note = history.Note,
                                       TransferDate = history.TransferDate
                                   }
            };
        }
    }
}
