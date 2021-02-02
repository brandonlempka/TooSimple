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

            if (!dataModel.Accounts.Any())
            {
                var emptyViewModel = new DashboardVM
                { 
                    Transactions = Enumerable.Empty<TransactionListVM>()
                };

                return emptyViewModel;
            }

            var accountGroup = dataModel.Accounts.GroupBy(x => x.AccessToken,
                x => x.AccountId,
                (key, y) => new { AccessToken = key, AccountIds = y.ToList() });

            var responseList = new List<StatusRM>();

            foreach (var token in accountGroup)
            {
                var ids = token.AccountIds.ToArray();

                var newResponse = await UpdateAccountDbAsync(userId, token.AccessToken, ids);
                responseList.Add(newResponse);
            }

            var updatedData = await _accountDataAccessor.GetAccountDMAsync(userId);

            var transactionList = new List<TransactionListVM>();

            foreach (var account in updatedData.Accounts)
            {
                var transaction = account.Transactions.EmptyIfNull().Select(x => new TransactionListVM
                {
                    AccountId = x.AccountId,
                    AccountName = account.Name,
                    AccountOwner = x.AccountOwner,
                    Address = x.Address,
                    Amount = x.Amount,
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
                    TransactionId = x.TransactionId,
                }).ToList();

                transactionList.AddRange(transaction);

            }

            var viewModel = new DashboardVM
            {
                CurrentBalance = updatedData.Accounts.Select(x => x.CurrentBalance).Sum(),
                Transactions = transactionList
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

            var refreshResponse = await UpdateAccountDbAsync(userId, responseModel.Access_token, accountIds);

            return refreshResponse;
        }

        private async Task<StatusRM> UpdateAccountDbAsync(string userId, string accessToken, string[] accountIds)
        {
            var genericError = "Something went wrong while contacting Plaid.";
            var account = await _plaidDataAccessor.GetAccountBalancesAsync(accessToken, accountIds);

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
            });

            var accountAddResponse = await _accountDataAccessor.SavePlaidAccountData(plaidAccount);

            if (!string.IsNullOrWhiteSpace(accountAddResponse.ErrorMessage))
            {
                return StatusRM.CreateError(genericError);
            }

            var transactionsRequest = new PlaidTransactionRequestModel
            {
                AccessToken = accessToken,
                StartDate = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd"),
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
                    NickName = account.NickName
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
                NickName = account.NickName
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

            return new DashboardGoalListVM
            {
                Goals = dataModel.Goals.Where(goal => goal.ExpenseFlag == isExpense)
                .Select(x => new DashboardGoalVM
                {
                    GoalAmount = x.GoalAmount,
                    UserAccountId = x.UserAccountId,
                    CurrentBalance = x.CurrentBalance,
                    DesiredCompletionDate = x.DesiredCompletionDate,
                    GoalId = x.GoalId,
                    GoalName = x.GoalName,
                    AmountNeededEachTimeFrame = x.AmountNeededEachTimeFrame,
                    ExpenseFlag = x.ExpenseFlag,
                    FirstCompletionDate = x.FirstCompletionDate,
                    RecurrenceTimeFrame = x.RecurrenceTimeFrame,
                })
            };

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
                CurrentBalance = existingAccount.CurrentBalance,
                DesiredCompletionDate = existingAccount.DesiredCompletionDate,
                GoalId = existingAccount.GoalId,
                GoalName = existingAccount.GoalName,
                UserAccountId = userId,
                AmountNeededEachTimeFrame = existingAccount.AmountNeededEachTimeFrame,
                ExpenseFlag = existingAccount.ExpenseFlag,
                FirstCompletionDate = existingAccount.FirstCompletionDate,
                FundingScheduleId = existingAccount.FundingScheduleId,
                RecurrenceTimeFrame = existingAccount.RecurrenceTimeFrame,
                FundingScheduleOptions = new List<SelectListItem>(),
                RecurrenceTimeFrameOptions = new List<SelectListItem>()
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
    }
}
