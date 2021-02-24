using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.Data;
using TooSimple.DataAccessors;
using TooSimple.Extensions;
using TooSimple.Misc.Enum;
using TooSimple.Models.ActionModels;
using TooSimple.Models.DataModels;
using TooSimple.Models.DataModels.Plaid;
using TooSimple.Models.RequestModels;
using TooSimple.Models.ResponseModels;
using TooSimple.Models.ResponseModels.Plaid;
using TooSimple.Models.ViewModels;

namespace TooSimple.Managers
{
    public class DashboardManager : IDashboardManager
    {
        private IAccountDataAccessor _accountDataAccessor;
        private IPlaidDataAccessor _plaidDataAccessor;
        private IBudgetingDataAccessor _budgetingDataAccessor;

        public DashboardManager(IAccountDataAccessor accountDataAccessor, IPlaidDataAccessor plaidDataAccessor, IBudgetingDataAccessor budgetingDataAccessor)
        {
            _accountDataAccessor = accountDataAccessor;
            _plaidDataAccessor = plaidDataAccessor;
            _budgetingDataAccessor = budgetingDataAccessor;
        }

        public async Task<DashboardVM> GetDashboardVMAsync(ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            var dataModel = await _accountDataAccessor.GetAccountDMAsync(userId);
            var includedAccounts = new AccountListDM
            {
                Accounts = dataModel.Accounts.Where(a => a.UseForBudgeting)
            };

            if (!dataModel.Accounts.Any())
            {
                var emptyViewModel = new DashboardVM
                {
                    Transactions = Enumerable.Empty<TransactionListVM>()
                };

                return emptyViewModel;
            }


            var responseList = new List<StatusRM>();

            var outdatedAccounts = dataModel.Accounts.Where(a => a.LastUpdated < DateTime.Now.AddMinutes(-15));

            if (outdatedAccounts.Any())
            {
                var accountGroup = dataModel.Accounts.GroupBy(x => x.AccessToken,
                    x => x.AccountId,
                    (key, y) => new { AccessToken = key, AccountIds = y.ToList() });

                var lastUpdated = outdatedAccounts.Min(a => a.LastUpdated)?.ToString("yyyy-MM-dd");

                foreach (var token in accountGroup)
                {
                    var ids = token.AccountIds.ToArray();
                    var newResponse = await UpdateAccountDbAsync(userId, token.AccessToken, ids, lastUpdated);
                    responseList.Add(newResponse);
                }
            }

            var goals = await _budgetingDataAccessor.GetGoalListDMAsync(userId);

            var transactionList = new List<TransactionListVM>();

            foreach (var account in dataModel.Accounts.Where(a => a.UseForBudgeting))
            {
                var transaction = account.Transactions.EmptyIfNull().Select(x => new TransactionListVM
                {
                    AccountId = x.AccountId,
                    AccountName = account.Name,
                    AccountOwner = x.AccountOwner,
                    Address = x.Address,
                    Amount = x.Amount,
                    AmountDisplayValue = x.Amount?.ToString("c") ?? "$0.00",
                    City = x.City,
                    Country = x.Country,
                    CurrencyCode = x.CurrencyCode,
                    InternalCategory = x.InternalCategory,
                    MerchantName = x.MerchantName,
                    Name = x.Name,
                    PaymentMethod = x.PaymentMethod,
                    Pending = x.Pending,
                    PostalCode = x.PostalCode,
                    Region = x.Region,
                    SpendingFrom = x.SpendingFrom,
                    TransactionCode = x.TransactionCode,
                    TransactionDate = x.TransactionDate,
                    TransactionDateDisplayValue = x.TransactionDate?.ToString("MM/dd/yyyy"),
                    TransactionId = x.TransactionId,
                }).ToList();

                transactionList.AddRange(transaction);

            }

            if (goals.Goals.Any())
            {
                await UpdateGoalFunding(userId, DateTime.Now);
            }

            foreach (var tran in transactionList)
            {
                var goalName = goals.Goals.EmptyIfNull().FirstOrDefault(g => g.GoalId == tran.SpendingFrom);
                if (goalName != null)
                {
                    tran.SpendingFrom = goalName.GoalName;
                }
                else
                {
                    tran.SpendingFrom = "Ready to Spend";
                }
            }

            var currentBalance = await _budgetingDataAccessor.CalculateUserAccountBalance(includedAccounts, userId);
            var viewModel = new DashboardVM
            {
                CurrentBalance = currentBalance,
                Transactions = transactionList.OrderByDescending(y => y.TransactionDate),
                LastUpdated = dataModel.Accounts.Max(a => a.LastUpdated)?.ToString("MM/dd/yyyy hh:mm")
            };

            var failures = responseList.Where(x => x.Success != true);
            if (failures.Any())
                viewModel.ErrorMessage = "Something went wrong while refreshing your accounts.";

            return viewModel;
        }

        public async Task<StatusRM> PublicTokenExchangeAsync(PublicTokenRM dataModel, ClaimsPrincipal currentUser)
        {
            var genericError = "Something went wrong while adding your account";
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (dataModel == null)
            {
                return StatusRM.CreateError(genericError);
            }

            var accountIds = dataModel.accounts.Select(x => x.id).ToArray();
            var responseModel = await _plaidDataAccessor.PublicTokenExchangeAsync(dataModel.public_token);

            if (string.IsNullOrWhiteSpace(responseModel.Access_token))
            {
                return StatusRM.CreateError(genericError);
            }

            var defaultTransactionsStart = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
            var refreshResponse = await UpdateAccountDbAsync(userId, responseModel.Access_token, accountIds, defaultTransactionsStart);

            return refreshResponse;
        }

        private async Task<StatusRM> UpdateAccountDbAsync(string userId, string accessToken, string[] accountIds, string lastUpdated)
        {
            var genericError = "Something went wrong while contacting Plaid.";
            var account = await _plaidDataAccessor.GetAccountBalancesAsync(accessToken, accountIds);
            var goals = await _budgetingDataAccessor.GetGoalListDMAsync(userId);
            var goalsList = goals.Goals.ToList();

            if (account == null)
            {
                return StatusRM.CreateError(genericError);
            }

            var plaidAccount = account.accounts.Select(x => new AccountDM
            {
                AccessToken = accessToken,
                UserAccountId = userId,
                AvailableBalance = x.balances.available,
                CurrentBalance = x.balances.current,
                AccountId = x.account_id,
                CurrencyCode = x.balances.iso_currency_code,
                Mask = x.mask,
                Name = x.name,
                LastUpdated = DateTime.Now,
                Transactions = Enumerable.Empty<TransactionDM>()
            });

            var accountAddResponse = await _accountDataAccessor.SavePlaidAccountData(plaidAccount);

            if (!string.IsNullOrWhiteSpace(accountAddResponse.ErrorMessage))
            {
                return StatusRM.CreateError(genericError);
            }

            var transactionsRequest = new PlaidTransactionRequestModel
            {
                AccessToken = accessToken,
                StartDate = lastUpdated,
                AccountIds = accountIds
            };

            var transactions = await _plaidDataAccessor.GetTransactionsAsync(transactionsRequest);
            var transactionsAddResponse = new StatusRM();

            var newTransactions = transactions.transactions.Select(x => new TransactionDM
            {
                AccountOwner = x.account_owner,
                Address = x.location.address,
                Amount = x.amount,
                AccountId = x.account_id,
                UserAccountId = userId,
                City = x.location.city,
                Country = x.location.country,
                CurrencyCode = x.iso_currency_code,
                MerchantName = x.merchant_name,
                Name = x.name,
                Pending = x.pending,
                PostalCode = x.location.postal_code,
                Region = x.location.region,
                TransactionDate = Convert.ToDateTime(x.date),
                TransactionCode = x.transaction_code,
                TransactionId = x.transaction_id
            }).ToList();

            //Auto spend
            foreach (var transaction in newTransactions)
            {
                var spendingFrom = goalsList.FirstOrDefault(g => g.AutoSpendMerchantName == transaction.MerchantName && g.AutoSpendMerchantName != null);
                if (spendingFrom != null)
                    transaction.SpendingFrom = spendingFrom.GoalId;
            }

            transactionsAddResponse = await _accountDataAccessor.SavePlaidTransactionData(newTransactions);

            if (transactionsAddResponse.Success && accountAddResponse.Success)
            {
                return StatusRM.CreateSuccess(null, "Successfully refreshed data.");
            }
            else
            {
                return StatusRM.CreateError(genericError);
            }
        }

        public async Task<DashboardAccountsVM> GetDashboardAccountsVMAsync(ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            var dataModel = await _accountDataAccessor.GetAccountDMAsync(userId);
            var linkToken = await _plaidDataAccessor.CreateLinkTokenAsync(userId);

            if (!dataModel.Accounts.Any())
            {
                return new DashboardAccountsVM
                {
                    Accounts = Enumerable.Empty<DashboardAccountsListVM>(),
                    LinkToken = linkToken.Link_Token
                };
            }

            var viewModel = new DashboardAccountsVM
            {
                LinkToken = linkToken.Link_Token,
                Accounts = dataModel.Accounts.Select(account => new DashboardAccountsListVM
                {
                    AccessToken = account.AccessToken,
                    AccountId = account.AccountId,
                    AvailableBalance = account.AvailableBalance,
                    CurrentBalance = account.CurrentBalance,
                    UserAccountId = account.UserAccountId,
                    CurrencyCode = account.CurrencyCode,
                    LastUpdated = account.LastUpdated,
                    Name = account.Name,
                    NickName = account.NickName ?? account.Name
                })
            };

            return viewModel;
        }

        public async Task<DashboardEditAccountVM> GetIndividualAccountVMAsync(string Id, ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            var dataModel = await _accountDataAccessor.GetAccountDMAsync(userId, Id);

            var account = dataModel.Accounts.FirstOrDefault();
            return new DashboardEditAccountVM
            {
                AccountId = account.AccountId,
                NickName = account.NickName ?? account.Name,
                AccountTypeId = account.AccountTypeId,
                AvailableBalance = account.AvailableBalance,
                CurrentBalance = account.CurrentBalance,
                CurrencyCode = account.CurrencyCode,
                LastUpdated = account.LastUpdated?.ToString("MM/dd/yyyy hh:mm"),
                Mask = account.Mask,
                Name = account.Name,
                UseForBudgeting = account.UseForBudgeting,
                Transactions = account.Transactions.EmptyIfNull().Select(t => new TransactionListVM
                {
                    AccountId = t.AccountId,
                    AccountName = account.NickName ?? account.Name,
                    AccountOwner = t.AccountOwner,
                    Address = t.Address,
                    Amount = t.Amount,
                    AmountDisplayValue = t.Amount?.ToString("c") ?? "$0.00",
                    City = t.City,
                    Country = t.Country,
                    CurrencyCode = t.CurrencyCode,
                    InternalCategory = t.InternalCategory,
                    MerchantName = t.MerchantName,
                    Name = t.Name,
                    PaymentMethod = t.PaymentMethod,
                    Pending = t.Pending,
                    PostalCode = t.PostalCode,
                    Region = t.Region,
                    SpendingFrom = t.SpendingFrom,
                    TransactionCode = t.TransactionCode,
                    TransactionDate = t.TransactionDate,
                    TransactionDateDisplayValue = t.TransactionDate?.ToString("MM/dd/yyyy"),
                    TransactionId = t.TransactionId,
                })
            };
        }

        public async Task<StatusRM> UpdateAccountAsync(DashboardSaveAccountAM actionModel)
        {
            return await _accountDataAccessor.UpdateAccountAsync(actionModel);
        }

        public async Task<StatusRM> DeleteAccountAsync(string accountId)
        {
            return await _accountDataAccessor.DeleteAccountAsync(accountId);
        }

        public async Task<DashboardGoalListVM> GetGoalsVMAsync(ClaimsPrincipal currentUser, bool isExpense = false)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var dataModel = await _budgetingDataAccessor.GetGoalListDMAsync(userId);
            var transactions = await _accountDataAccessor.GetSpendingFromTransactions(userId);

            var viewModel = new DashboardGoalListVM
            {
                Goals = dataModel.Goals.Where(goal => goal.ExpenseFlag == isExpense)
                .Select(x => new DashboardGoalVM
                {
                    GoalAmount = x.GoalAmount.ToString("c"),
                    UserAccountId = x.UserAccountId,
                    CurrentBalance = (x.CurrentBalance - (transactions.Transactions.EmptyIfNull().Where(t => t.SpendingFrom == x.GoalId).Sum(y => y.Amount) ?? 0)).ToString("c"),
                    DesiredCompletionDate = x.DesiredCompletionDate,
                    GoalId = x.GoalId,
                    GoalName = x.GoalName,
                    ExpenseFlag = x.ExpenseFlag,
                    RecurrenceTimeFrame = x.RecurrenceTimeFrame,
                    Paused = x.Paused,
                    ProgressPercent = (x.CurrentBalance - (transactions.Transactions.EmptyIfNull().Where(t => t.SpendingFrom == x.GoalId).Sum(y => y.Amount)) == 0 ? "0%" 
                        :  (x.GoalAmount / x.CurrentBalance - (transactions.Transactions.EmptyIfNull().Where(t => t.SpendingFrom == x.GoalId).Sum(y => y.Amount) ?? 0)).ToString() + "%")
                })
            };

            return viewModel;
        }

        public async Task<DashboardSaveGoalVM> GetSaveGoalVMAsync(string goalId, ClaimsPrincipal currentUser, bool isExpense)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var fundingSchedules = await _budgetingDataAccessor.GetFundingScheduleListDMAsync(userId);
            var expenseSchedules = new List<ExpenseFrequencies>
            {
                ExpenseFrequencies.Weekly,
                ExpenseFrequencies.BiWeekly,
                ExpenseFrequencies.Monthly,
                ExpenseFrequencies.BiMonthly,
                ExpenseFrequencies.ThreeMonths,
                ExpenseFrequencies.SixMonths,
                ExpenseFrequencies.Yearly
            };

            if (string.IsNullOrWhiteSpace(goalId))
                return new DashboardSaveGoalVM
                {
                    UserAccountId = userId,
                    FundingScheduleOptions = fundingSchedules.FundingSchedules.Select(schedule => new SelectListItem
                    {
                        Text = schedule.FundingScheduleName,
                        Value = schedule.FundingScheduleId,
                        Selected = false
                    }).ToList(),
                    RecurrenceTimeFrameOptions = expenseSchedules.Select(option => new SelectListItem
                    {
                        Text = option.ToString(),
                        Value = ((int)option).ToString(),
                        Selected = false
                    }).ToList(),
                    ExpenseFlag = isExpense
                };

            var existingAccount = await _budgetingDataAccessor.GetGoalDMAsync(goalId);

            var viewModel = new DashboardSaveGoalVM
            {
                GoalAmount = existingAccount.GoalAmount,
                DesiredCompletionDate = existingAccount.DesiredCompletionDate,
                GoalId = existingAccount.GoalId,
                GoalName = existingAccount.GoalName,
                UserAccountId = userId,
                ExpenseFlag = existingAccount.ExpenseFlag,
                FundingScheduleId = existingAccount.FundingScheduleId,
                RecurrenceTimeFrame = existingAccount.RecurrenceTimeFrame,
                FundingScheduleOptions = new List<SelectListItem>(),
                RecurrenceTimeFrameOptions = new List<SelectListItem>(),
                Paused = existingAccount.Paused,
                AutoSpendMerchantName = existingAccount.AutoSpendMerchantName
            };

            if (fundingSchedules.FundingSchedules.Any())
            {
                foreach (var schedule in fundingSchedules.FundingSchedules)
                {
                    if (schedule.FundingScheduleId == existingAccount.FundingScheduleId)
                    {
                        viewModel.FundingScheduleOptions.Add(new SelectListItem
                        {
                            Selected = true,
                            Value = existingAccount.FundingScheduleId,
                            Text = schedule.FundingScheduleName
                        });
                    }
                    else
                    {
                        viewModel.FundingScheduleOptions.Add(new SelectListItem
                        {
                            Selected = false,
                            Value = existingAccount.FundingScheduleId,
                            Text = schedule.FundingScheduleName
                        });
                    }
                }
            }

            foreach (var option in expenseSchedules)
            {
                if ((int)option == existingAccount.RecurrenceTimeFrame)
                {
                    viewModel.RecurrenceTimeFrameOptions.Add(new SelectListItem
                    {
                        Selected = true,
                        Value = ((int)option).ToString(),
                        Text = option.ToString()
                    });
                }
                else
                {
                    viewModel.RecurrenceTimeFrameOptions.Add(new SelectListItem
                    {
                        Selected = false,
                        Value = ((int)option).ToString(),
                        Text = option.ToString()
                    });
                }
            }

            return viewModel;
        }

        public async Task<StatusRM> UpdateGoalAsync(DashboardSaveGoalAM actionModel)
        {
            return await _budgetingDataAccessor.SaveGoalAsync(actionModel);
        }

        public async Task<StatusRM> DeleteGoalAsync(string goalId)
        {
            return await _budgetingDataAccessor.DeleteGoalAsync(goalId);
        }

        public async Task<DashboardEditTransactionVM> GetEditTransactionVMAsync(string transactionId, ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var goals = await _budgetingDataAccessor.GetGoalListDMAsync(userId);

            var dataModel = await _accountDataAccessor.GetTransactionDMAsync(transactionId);

            var viewModel = new DashboardEditTransactionVM
            {
                Goals = new List<SelectListItem>(),
                AccountId = dataModel.AccountId,
                AccountOwner = dataModel.AccountOwner,
                Address = dataModel.Address,
                Amount = dataModel.Amount.GetValueOrDefault(),
                UserAccountId = dataModel.UserAccountId,
                City = dataModel.City,
                Country = dataModel.Country,
                CurrencyCode = dataModel.CurrencyCode,
                TransactionDate = dataModel.TransactionDate,
                InternalCategory = dataModel.InternalCategory,
                MerchantName = dataModel.MerchantName,
                Name = dataModel.Name,
                Pending = dataModel.Pending,
                TransactionId = dataModel.TransactionId,
            };

            if (goals.Goals.Any())
            {

                viewModel.Goals = goals.Goals.Select(goal => new SelectListItem
                {
                    Text = goal.GoalName,
                    Value = goal.GoalId,
                }).ToList();

            }

            //Add ready to spend (null) option.
            if (!string.IsNullOrWhiteSpace(dataModel.SpendingFrom))
            {

                foreach (var goal in viewModel.Goals.Where(x => x.Value == dataModel.SpendingFrom))
                {
                    goal.Selected = true;
                }

                viewModel.Goals.Add(new SelectListItem
                {
                    Text = "Ready to Spend",
                    Value = string.Empty,
                });

            }
            else
            {
                viewModel.Goals.Add(new SelectListItem
                {
                    Text = "Ready to Spend",
                    Value = string.Empty,
                    Selected = true
                });
            }

            return viewModel;
        }

        public async Task<StatusRM> UpdateTransactionAsync(DashboardEditTransactionAM actionModel)
        {
            if (string.IsNullOrWhiteSpace(actionModel.SpendingFromId))
                actionModel.SpendingFromId = null;
            return await _accountDataAccessor.SaveTransactionAsync(actionModel);
        }

        public async Task<DashboardFundingScheduleListVM> GetDashboardFundingScheduleListVM(ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            var dataModel = await _budgetingDataAccessor.GetFundingScheduleListDMAsync(userId);

            return new DashboardFundingScheduleListVM
            {
                FundingSchedules = dataModel.FundingSchedules.Select(schedule => new DashboardFundingScheduleVM
                {
                    UserAccountId = schedule.UserAccountId,
                    FirstContributionDate = schedule.FirstContributionDate,
                    Frequency = schedule.Frequency,
                    FundingScheduleId = schedule.FundingScheduleId,
                    FundingScheduleName = schedule.FundingScheduleName
                })
            };
        }

        public async Task<DashboardFundingScheduleVM> GetDashboardFundingScheduleVM(string scheduleId, ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            var frequencyOptions = new List<FundingFrequencies>
            {
                FundingFrequencies.Weekly,
                FundingFrequencies.BiWeekly,
                FundingFrequencies.Monthly,
                FundingFrequencies.BiMonthly
            };

            if (string.IsNullOrWhiteSpace(scheduleId))
            {
                var newModel = new DashboardFundingScheduleVM
                {
                    UserAccountId = userId,
                    FrequencyList = frequencyOptions.Select(option => new SelectListItem
                    {
                        Text = option.ToString(),
                        Value = ((int)option).ToString(),
                        Selected = false
                    }).ToList()
                };

                return newModel;
            }

            var dataModel = await _budgetingDataAccessor.GetFundingScheduleDMAsync(scheduleId);

            var viewModel = new DashboardFundingScheduleVM
            {
                UserAccountId = dataModel.UserAccountId,
                FirstContributionDate = dataModel.FirstContributionDate,
                Frequency = dataModel.Frequency,
                FundingScheduleId = dataModel.FundingScheduleId,
                FundingScheduleName = dataModel.FundingScheduleName,
                FrequencyList = new List<SelectListItem>()
            };

            foreach (var option in frequencyOptions)
            {
                if ((int)option == dataModel.Frequency)
                {
                    viewModel.FrequencyList.Add(new SelectListItem
                    {
                        Text = option.ToString(),
                        Value = ((int)option).ToString(),
                        Selected = true
                    });
                }
                else
                {
                    viewModel.FrequencyList.Add(new SelectListItem
                    {
                        Text = option.ToString(),
                        Value = ((int)option).ToString(),
                        Selected = false
                    });
                }
            }

            return viewModel;
        }

        public async Task<StatusRM> UpdateFundingScheduleAsync(DashboardSaveFundingScheduleAM actionModel)
        {
            return await _budgetingDataAccessor.SaveScheduleAsync(actionModel);
        }

        public async Task<StatusRM> DeleteFundingScheduleAsync(string scheduleId)
        {
            return await _budgetingDataAccessor.DeleteScheduleAsync(scheduleId);
        }

        public async Task<DashboardMoveMoneyVM> GetMoveMoneyVMAsync(ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            var goalsList = await _budgetingDataAccessor.GetGoalListDMAsync(userId);

            var viewModel = new DashboardMoveMoneyVM
            {
                AccountsList = new List<SelectListItem>(),
                UserAccountId = userId
            };

            viewModel.AccountsList = goalsList.Goals.EmptyIfNull().Select(goal => new SelectListItem
            {
                Text = goal.GoalName,
                Value = goal.GoalId,
                Selected = false
            }).ToList();

            viewModel.AccountsList.Add(new SelectListItem
            {
                Text = "Ready to Spend",
                Value = "0",
                Selected = true
            });

            return viewModel;
        }

        public async Task<StatusRM> SaveMoveMoneyAsync(DashboardMoveMoneyAM actionModel)
        {
            return await _budgetingDataAccessor.SaveMoveMoneyAsync(actionModel);
        }

        public async Task UpdateGoalFunding(string userId, DateTime todayDateTime)
        {
            var goals = await _budgetingDataAccessor.GetGoalListDMAsync(userId);
            var today = todayDateTime.Date;
            var schedules = await _budgetingDataAccessor.GetFundingScheduleListDMAsync(userId);

            //Goal calculation
            foreach (var goal in goals.Goals.Where(g => !g.Paused).ToList())
            {
                if (goal.DesiredCompletionDate > today || goal.ExpenseFlag)
                {
                    foreach (var schedule in schedules.FundingSchedules.ToList())
                    {
                        if (schedule.FundingScheduleId == goal.FundingScheduleId)
                        {
                            var fundingHistory = await _budgetingDataAccessor.GetFundingHistoryListDMAsync(goal.GoalId);
                            var goalHistory = fundingHistory.FundingHistories.EmptyIfNull()
                                .Where(g => g.ToAccountId == goal.GoalId && g.AutomatedTransfer == true)
                                .OrderByDescending(g => g.TransferDate)
                                .ToList();

                            var lastFunded = todayDateTime;
                            if (goalHistory.Any())
                            {
                                lastFunded = goalHistory.Max(g => g.TransferDate);
                            }
                            else
                            {
                                lastFunded = goal.CreationDate;
                            }

                            var nextContribution = todayDateTime;

                            nextContribution = CalculateNextGoalContributionDate(lastFunded, schedule.FirstContributionDate, schedule.Frequency);

                            while (nextContribution > lastFunded && nextContribution <= today)
                            {
                                int contributionsRemaining = 0;

                                var nextExpenseDate = goal.DesiredCompletionDate;

                                if (!goal.ExpenseFlag || goal.DesiredCompletionDate > today)
                                    contributionsRemaining = CalculateContributionsToComplete(goal.DesiredCompletionDate, lastFunded, schedule.Frequency);
                                else
                                {
                                    nextExpenseDate = CalculateNextExpenseContributionDate(today, nextExpenseDate, goal.RecurrenceTimeFrame ?? 0);
                                    contributionsRemaining = CalculateContributionsToComplete(nextExpenseDate, lastFunded, schedule.Frequency);
                                }

                                var nextAmount = Math.Round((goal.GoalAmount - goal.CurrentBalance) / contributionsRemaining, 2);

                                var actionModel = new DashboardMoveMoneyAM
                                {
                                    Amount = nextAmount,
                                    FromAccountId = "0",
                                    ToAccountId = goal.GoalId,
                                    UserAccountId = userId,
                                    Note = "Automatic funding from " + schedule.FundingScheduleName,
                                    AutomatedTransfer = true,
                                    TransferDate = nextContribution,
                                };

                                var response = await _budgetingDataAccessor.SaveMoveMoneyAsync(actionModel);

                                if (response.Success)
                                {
                                    nextContribution = CalculateNextGoalContributionDate(nextContribution, schedule.FirstContributionDate, schedule.Frequency);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public int CalculateContributionsToComplete(DateTime completionDate, DateTime lastContributed, int frequency)
        {
            var nextContribution = lastContributed;
            var numberOfContributionsRemaining = 0;

            switch (frequency)
            {
                case 1:
                    while (completionDate > nextContribution)
                    {
                        nextContribution = nextContribution.AddDays(7);
                        numberOfContributionsRemaining++;
                    }
                    break;
                case 2:
                    while (completionDate > nextContribution)
                    {
                        nextContribution = nextContribution.AddDays(14);
                        numberOfContributionsRemaining++;
                    }
                    break;
                case 3:
                    while (completionDate > nextContribution)
                    {
                        nextContribution = nextContribution.AddMonths(1);
                        numberOfContributionsRemaining++;
                    }
                    break;
                case 4:
                    while (completionDate > nextContribution)
                    {
                        nextContribution = nextContribution.AddMonths(2);
                        numberOfContributionsRemaining++;
                    }
                    break;
                default:
                    return 0;
            }

            return numberOfContributionsRemaining;


            //var dateDiff = 0;

            //switch (frequency)
            //{
            //    case 1:
            //        dateDiff = (completionDate - lastContributed).Days;
            //        return dateDiff / 7;
            //    case 2:
            //        dateDiff = (completionDate - lastContributed).Days;
            //        return dateDiff / 14;
            //    case 3:
            //        dateDiff = (completionDate.Month - lastContributed.Month);
            //        return dateDiff;
            //    case 4:
            //        dateDiff = (completionDate.Month - lastContributed.Month);
            //        return dateDiff / 2;
            //    //to do
            //    //case 5:
            //    //    dateDiff = ()
            //    default:
            //        return 0;
            //}
        }

        public DateTime CalculateNextGoalContributionDate(DateTime lastFunded, DateTime scheduleFirstDate, int frequency)
        {
            DateTime nextContribution;

            if (lastFunded.Date >= scheduleFirstDate)
            {
                nextContribution = lastFunded.Date;
                while (nextContribution >= scheduleFirstDate)
                {
                    switch (frequency)
                    {
                        case 1:
                            scheduleFirstDate = scheduleFirstDate.AddDays(7).Date;
                            break;
                        case 2:
                            scheduleFirstDate = scheduleFirstDate.AddDays(14).Date;
                            break;
                        case 3:
                            scheduleFirstDate = scheduleFirstDate.AddMonths(1).Date;
                            break;
                        case 4:
                            scheduleFirstDate = scheduleFirstDate.AddMonths(2).Date;
                            break;
                            //to do
                            //case 5:
                            //    var newMonth = scheduleFirstDate.AddDays(1);
                            //    nextContribution = new DateTime(newMonth.Day,
                            //        newMonth.Month,
                            //        DateTime.DaysInMonth(newMonth.AddDays(1).Year, newMonth.Month));
                            //break;
                    }
                }

                nextContribution = scheduleFirstDate;
                return nextContribution;
            }

            return scheduleFirstDate;
        }

        public DateTime CalculateNextExpenseContributionDate(DateTime today, DateTime nextExpenseDate, int recurrence)
        {
            if (recurrence == 1)
            {
                while (today >= nextExpenseDate)
                {
                    nextExpenseDate = nextExpenseDate.AddDays(7);
                }
            }
            else if (recurrence == 2)
            {
                while (today >= nextExpenseDate)
                {
                    nextExpenseDate = nextExpenseDate.AddDays(14);
                }
            }
            else if (recurrence == 3)
            {
                while (today >= nextExpenseDate)
                {
                    nextExpenseDate = nextExpenseDate.AddMonths(1);
                }
            }
            else if (recurrence == 4)
            {
                while (today >= nextExpenseDate)
                {
                    nextExpenseDate = nextExpenseDate.AddMonths(2);
                }
            }
            else if (recurrence == 5)
            {
                while (today >= nextExpenseDate)
                {
                    nextExpenseDate = nextExpenseDate.AddMonths(3);
                }
            }
            else if (recurrence == 6)
            {
                while (today >= nextExpenseDate)
                {
                    nextExpenseDate = nextExpenseDate.AddMonths(6);
                }
            }
            else if (recurrence == 7)
            {
                while (today >= nextExpenseDate)
                {
                    nextExpenseDate = nextExpenseDate.AddYears(1);
                }
            }

            return nextExpenseDate;
        }
    }
}