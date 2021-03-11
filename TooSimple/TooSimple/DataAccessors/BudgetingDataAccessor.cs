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
using TooSimple.Models.RequestModels;
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
                            AmountContributed = goal.AmountContributed,
                            DesiredCompletionDate = goal.DesiredCompletionDate,
                            GoalId = goal.GoalId,
                            GoalName = goal.GoalName,
                            FundingScheduleId = goal.FundingScheduleId,
                            ExpenseFlag = goal.ExpenseFlag,
                            RecurrenceTimeFrame = goal.RecurrenceTimeFrame,
                            CreationDate = goal.CreationDate,
                            Paused = goal.Paused,                
                            AutoSpendMerchantName = goal.AutoSpendMerchantName,
                            AmountSpent = goal.AmountSpent,
                            AutoRefill = goal.AutoRefill,
                            NextContributionAmount = goal.NextContributionAmount,
                            NextContributionDate = goal.NextContributionDate
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
                AmountContributed = data.AmountContributed,
                DesiredCompletionDate = data.DesiredCompletionDate,
                GoalId = data.GoalId,
                GoalName = data.GoalName,
                FundingScheduleId = data.FundingScheduleId,
                UserAccountId = data.UserAccountId,
                ExpenseFlag = data.ExpenseFlag,
                RecurrenceTimeFrame = data.RecurrenceTimeFrame,
                CreationDate = data.CreationDate,
                Paused = data.Paused,
                AutoSpendMerchantName = data.AutoSpendMerchantName,
                AmountSpent = data.AmountSpent,
                AutoRefill = data.AutoRefill,
                NextContributionAmount = data.NextContributionAmount,
                NextContributionDate = data.NextContributionDate
            };
        }

        public async Task<StatusRM> SaveGoalAsync(GoalDM dataModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dataModel.GoalId))
                {
                    await _db.Goals.AddAsync(new Goal
                    {
                        GoalAmount = dataModel.GoalAmount,
                        AmountContributed = 0,
                        DesiredCompletionDate = dataModel.DesiredCompletionDate,
                        GoalName = dataModel.GoalName,
                        UserAccountId = dataModel.UserAccountId,
                        FundingScheduleId = dataModel.FundingScheduleId,
                        ExpenseFlag = dataModel.ExpenseFlag,
                        RecurrenceTimeFrame = dataModel.RecurrenceTimeFrame,
                        CreationDate = dataModel.CreationDate,
                        Paused = dataModel.Paused,
                        AutoSpendMerchantName = dataModel.AutoSpendMerchantName,
                        AutoRefill = dataModel.AutoRefill,
                        NextContributionAmount = dataModel.NextContributionAmount,
                        NextContributionDate = dataModel.NextContributionDate
                    });

                    await _db.SaveChangesAsync();

                    return StatusRM.CreateSuccess(null, "Success");
                }

                var existingGoal = await _db.Goals.FirstOrDefaultAsync(x => x.GoalId == dataModel.GoalId);

                existingGoal.GoalName = dataModel.GoalName;
                existingGoal.GoalAmount = dataModel.GoalAmount;
                existingGoal.DesiredCompletionDate = dataModel.DesiredCompletionDate;
                existingGoal.FundingScheduleId = dataModel.FundingScheduleId;
                existingGoal.RecurrenceTimeFrame = dataModel.RecurrenceTimeFrame;
                existingGoal.ExpenseFlag = dataModel.ExpenseFlag;
                existingGoal.Paused = dataModel.Paused;
                existingGoal.AutoSpendMerchantName = dataModel.AutoSpendMerchantName;
                existingGoal.AutoRefill = dataModel.AutoRefill;
                existingGoal.NextContributionDate = dataModel.NextContributionDate;
                existingGoal.NextContributionAmount = dataModel.NextContributionAmount;

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

        public async Task<StatusRM> SaveMoveMoneyAsync(MoveMoneyRequestModel requestModel)
        {
            try
            {
                if (requestModel.FromAccountId == "0")
                {
                    var existingGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == requestModel.ToAccountId);
                    existingGoal.AmountContributed += requestModel.Amount;
                    await _db.SaveChangesAsync();
                }
                else if (requestModel.ToAccountId == "0")
                {
                    var existingGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == requestModel.FromAccountId);
                    if (requestModel.AutoRefill)
                    {
                        existingGoal.AmountContributed -= requestModel.Amount;
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        existingGoal.AmountSpent += requestModel.Amount;
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    var fromGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == requestModel.FromAccountId);
                    var toGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == requestModel.ToAccountId);

                    if (requestModel.AutoRefill)
                    {
                        fromGoal.AmountContributed -= requestModel.Amount;
                    }
                    else
                    {
                        fromGoal.AmountSpent += requestModel.Amount;
                    }

                    toGoal.AmountContributed += requestModel.Amount;
                    await _db.SaveChangesAsync();
                }






                //if (requestModel.FromAccountId == "0" && requestModel.AutoRefill)
                //{
                //    var existingGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == requestModel.ToAccountId);
                //    existingGoal.AmountContributed += requestModel.Amount;
                //    await _db.SaveChangesAsync();
                //}
                //else if (requestModel.FromAccountId != "0" && !requestModel.AutoRefill)
                //{
                //    var fromGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == requestModel.FromAccountId);
                //    var toGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == requestModel.ToAccountId);

                //    fromGoal.AmountSpent += requestModel.Amount;
                //    toGoal.AmountContributed += requestModel.Amount;
                //    await _db.SaveChangesAsync();
                //}
                //else if (requestModel.ToAccountId == "0" && requestModel.AutoRefill)
                //{
                //    var existingGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == requestModel.FromAccountId);
                //    existingGoal.AmountContributed =- requestModel.Amount;
                //    await _db.SaveChangesAsync();
                //}
                //else
                //{
                //    var fromGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == requestModel.FromAccountId);
                //    var toGoal = await _db.Goals.FirstOrDefaultAsync(goal => goal.GoalId == requestModel.ToAccountId);

                //    fromGoal.AmountContributed -= requestModel.Amount;
                //    toGoal.AmountContributed += requestModel.Amount;
                //    await _db.SaveChangesAsync();
                //}


                var historyEntry = new FundingHistory
                {
                    Amount = requestModel.Amount,
                    FromAccountId = requestModel.FromAccountId,
                    ToAccountId = requestModel.ToAccountId,
                    Note = requestModel.Note,
                    TransferDate = requestModel.TransferDate,
                    AutomatedTransfer = requestModel.AutomatedTransfer,
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
            var goalsSum = goalDM.Goals.EmptyIfNull().Select(x => x.AmountContributed).Sum();

            accountSum -= goalsSum;
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
