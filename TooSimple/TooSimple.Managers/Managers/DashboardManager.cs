using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.DataAccessors;
using TooSimple.DataAccessors.Plaid;
using TooSimple.DataAccessors.TooSimple;
using TooSimple.Managers.Extensions;
using TooSimple.Managers.Misc.Enum;
using TooSimple.Models.ActionModels;
using TooSimple.Poco.Models.ActionModels;
using TooSimple.Poco.Models.DataModels;
using TooSimple.Poco.Models.RequestModels;
using TooSimple.Poco.Models.ResponseModels;
using TooSimple.Poco.Models.ResponseModels.Plaid;
using TooSimple.Poco.Models.ViewModels;
using TooSimple.Poco.Extensions;
using TooSimple.Poco.Enum;

namespace TooSimple.Managers.Managers
{
    public class DashboardManager : IDashboardManager
    {
        private IAccountDataAccessor _accountDataAccessor;
        private IPlaidDataAccessor _plaidDataAccessor;
        private IBudgetingDataAccessor _budgetingDataAccessor;
        private string _relogError = "Your credentials have expired for some of your accounts. Head to the Accounts page to fix this.";

        public DashboardManager(IAccountDataAccessor accountDataAccessor, IPlaidDataAccessor plaidDataAccessor, IBudgetingDataAccessor budgetingDataAccessor)
        {
            _accountDataAccessor = accountDataAccessor;
            _plaidDataAccessor = plaidDataAccessor;
            _budgetingDataAccessor = budgetingDataAccessor;
        }

        private async Task<StatusRM> UpdateAccountDbAsync(string userId, string accessToken, string[] accountIds, string lastUpdated)
        {
            var genericError = "Something went wrong while contacting Plaid.";
            var account = await _plaidDataAccessor.GetAccountBalancesAsync(accessToken, accountIds);
            
            //If error, disable this account
            if (!string.IsNullOrWhiteSpace(account.error_code))
            {
                foreach (var lockedAccount in accountIds)
                {
                    await _accountDataAccessor.SetRelogAsync(lockedAccount);
                }

                return StatusRM.CreateError(_relogError);
            }

            var goals = await _budgetingDataAccessor.GetGoalListDMAsync(userId);
            var goalsList = goals.Goals.ToList();

            if (account == null)
            {
                return StatusRM.CreateError(genericError);
            }

            var plaidAccount = account.accounts.Select(x => new AccountDM
            {
                AccessToken = accessToken,
                //fix this to be able to be 0/unknown
                AccountTypeId = (AccountType)(x.subtype == "checking" ? 1 : 2),
                UserAccountId = userId,
                AvailableBalance = x.balances.available,
                CurrentBalance = x.balances.current,
                AccountId = x.account_id,
                CurrencyCode = x.balances.iso_currency_code,
                Mask = x.mask,
                Name = x.name,
                LastUpdated = DateTime.UtcNow,
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
                //to do
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

        public async Task<DashboardVM> GetDashboardVMAsync(ClaimsPrincipal currentUser, string apiUserId = "")
        { 
            var userId = !string.IsNullOrWhiteSpace(apiUserId)
                ? apiUserId
                : currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var dataModel = await _accountDataAccessor.GetAccountDMAsync(userId);

            if (!dataModel.Accounts.Any())
            {
                var emptyViewModel = new DashboardVM
                {
                    TransactionTableVM = new TransactionTableVM
                    {
                        Transactions = Enumerable.Empty<TransactionListVM>()
                    }
                };

                return emptyViewModel;
            }

            var includedAccounts = new AccountListDM
            {
                Accounts = dataModel.Accounts.Where(a => a.UseForBudgeting)
            };

            var responseList = new List<StatusRM>();

            var outdatedAccounts = dataModel.Accounts.Where(a => a.LastUpdated < DateTime.UtcNow.AddMinutes(-15) && !a.ReLoginRequired);

            //if (outdatedAccounts.Any())
            //{
            //    var accountGroup = dataModel.Accounts.Where(y => !y.ReLoginRequired).GroupBy(x => x.AccessToken,
            //        x => x.AccountId,
            //        (key, y) => new { AccessToken = key, AccountIds = y.ToList() });

            //    var lastUpdated = outdatedAccounts.Min(a => a.LastUpdated)?.ToString("yyyy-MM-dd");

            //    foreach (var token in accountGroup)
            //    {
            //        var ids = token.AccountIds.ToArray();
            //        var newResponse = await UpdateAccountDbAsync(userId, token.AccessToken, ids, lastUpdated);
            //        responseList.Add(newResponse);
            //    }
            //}

            var goals = await _budgetingDataAccessor.GetGoalListDMAsync(userId);

            var transactionList = new List<TransactionListVM>();

            foreach (var account in dataModel.Accounts.Where(a => a.UseForBudgeting))
            {
                var transactions = account.Transactions.EmptyIfNull().Select(x => new TransactionListVM(x)).ToList();

                transactionList.AddRange(transactions);
            }

            if (goals.Goals.Any())
            {
                await UpdateGoalFunding(userId, DateTime.UtcNow);
            }

            //foreach (var tran in transactionList.EmptyIfNull())
            //{
            //    var goalName = goals.Goals.EmptyIfNull().FirstOrDefault(g => g.GoalId == tran.SpendingFrom);
            //    if (goalName != null)
            //    {
            //        tran.SpendingFrom = goalName.GoalName;
            //    }
            //    else
            //    {
            //        tran.SpendingFrom = "Ready to Spend";
            //    }

            //    if (tran.Amount < 0)
            //        tran.AmountDisplayColor = "#ff0000";
            //    else
            //        tran.AmountDisplayColor = "#32CD32";
            //}

            var currentBalance = await _budgetingDataAccessor.CalculateUserAccountBalance(includedAccounts, userId);
            var viewModel = new DashboardVM
            {
                CurrentBalance = currentBalance,
                AmountDisplayValue = currentBalance?.ToString("c") ?? "$0.00",
                TransactionTableVM = await GetTransactionTableVMAsync(currentUser, 1, apiUserId),
                LastUpdated = dataModel.Accounts.Max(a => a.LastUpdated)?.DateToCentral().ToString("MM/dd/yyyy hh:mm tt")
            };

            if (currentBalance < 0)
            {
                viewModel.AmountDisplayColor = "#ff0000";
            }

            var failures = responseList.Where(x => x.Success != true);
            if (failures.Any())
                viewModel.ErrorMessage = string.Concat(responseList.SelectMany(x => x.ErrorMessage));

            var expiredLogins = dataModel.Accounts.Where(a => a.ReLoginRequired);

            if (expiredLogins.Any())
            {
                viewModel.ErrorMessage += _relogError;
            }

            if (outdatedAccounts.Any())
                viewModel.NeedsUpdating = true;
            else
                viewModel.NeedsUpdating = false;
            return viewModel;
        }

        public async Task<TransactionTableVM> GetTransactionTableVMAsync(ClaimsPrincipal currentUser, int pageNumber = 1, string apiUserId = "")
        {
            var userId = !string.IsNullOrWhiteSpace(apiUserId)
                ? apiUserId
                : currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var resultsPerPage = 25;
            var accountList = await _accountDataAccessor.GetTransactionListAsync(userId, pageNumber, resultsPerPage);
            var transactions = accountList.Transactions.Select(x => new TransactionListVM(x));
            //var transactions = accountList.SelectMany(x => x.Transactions.Select(y => new TransactionListVM(y, x.NickName)));
            var maxPages = Convert.ToInt32(Math.Ceiling(accountList.TransactionCount / (decimal)resultsPerPage));

            return new TransactionTableVM
            {
                Transactions = transactions,
                PagerVM = new PagerVM(pageNumber, maxPages,  $"/Dashboard/GetTransactionTablePage?page=")
                {
                    IsAjaxPager = true
                }
            };

        }

        /// <summary>
        /// Called via ajax to update accounts from plaid without needing to make user wait
        /// </summary>
        /// <param name="currentUser">current User provided by controller</param>
        /// <returns></returns>
        public async Task<StatusRM> UpdatePlaidAccountDataAsync(ClaimsPrincipal? currentUser = null)
        {
            var userId = string.Empty;
            if (currentUser != null)
            {
                userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            else
            {
                userId = "1d4c76c2-148b-47b5-9a53-c29f3a233c80";
            }

            var dataModel = await _accountDataAccessor.GetAccountDMAsync(userId); 
            var outdatedAccounts = dataModel.Accounts.Where(a => a.LastUpdated < DateTime.UtcNow.AddMinutes(-15) && !a.ReLoginRequired);
            
            var responseList = new List<StatusRM>();
            var accountGroup = outdatedAccounts.Where(y => !y.ReLoginRequired).GroupBy(x => x.AccessToken,
                x => x.AccountId,
                (key, y) => new { AccessToken = key, AccountIds = y.ToList() });

            var lastUpdated = outdatedAccounts.Min(a => a.LastUpdated);
            var lastUpdatedString = lastUpdated?.ToString("yyyy-MM-dd");

            foreach (var token in accountGroup)
            {
                var ids = token.AccountIds.ToArray();
                var newResponse = await UpdateAccountDbAsync(userId, token.AccessToken, ids, lastUpdatedString);
                responseList.Add(newResponse);
            }

            var goals = await _budgetingDataAccessor.GetGoalListDMAsync(userId);

            var transactionList = new List<TransactionListVM>();

            foreach (var account in outdatedAccounts.Where(a => a.UseForBudgeting))
            {
                var transaction = account.Transactions.EmptyIfNull().Where(t => t.TransactionDate > lastUpdated).Select(x => new TransactionListVM(x));

                transactionList.AddRange(transaction);

            }

            foreach (var tran in transactionList.EmptyIfNull())
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

                if (tran.Amount < 0)
                    tran.AmountDisplayColor = "#ff0000";
                else
                    tran.AmountDisplayColor = "#32CD32";
            }

            var updatedAccounts = await _accountDataAccessor.GetAccountDMAsync(userId);
            var includedAccounts = new AccountListDM
            {
                Accounts = updatedAccounts.Accounts.Where(a => a.UseForBudgeting)
            };

            var currentBalance = await _budgetingDataAccessor.CalculateUserAccountBalance(includedAccounts, userId);
            var failures = responseList.Where(x => x.Success != true);

            if (failures.Any())
                return StatusRM.CreateError(string.Concat(failures.SelectMany(x => x.ErrorMessage)));

            return StatusRM.CreateSuccess(null, "Success");

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

            var defaultTransactionsStart = DateTime.UtcNow.AddDays(-90).ToString("yyyy-MM-dd");
            var refreshResponse = await UpdateAccountDbAsync(userId, responseModel.Access_token, accountIds, defaultTransactionsStart);

            return refreshResponse;
        }

        public async Task<StatusRM> PublicTokenUpdateAsync(PublicTokenUpdateAM actionModel, ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (actionModel.public_token == "Error")
            {
                return StatusRM.CreateError(actionModel.public_token);
            }

            var accounts = await _accountDataAccessor.GetAccountDMAsync(userId);
            var account = accounts.Accounts.Where(x => x.AccountId == actionModel.accountId).FirstOrDefault();
            var unsetAccounts = accounts.Accounts.Where(x => x.AccessToken == account.AccessToken);

            if (account == null)
            {
                return StatusRM.CreateError("Something went wrong");
            }

            var responseList = new List<StatusRM>();

            foreach (var unsetAccount in unsetAccounts)
            {
                var response = await _accountDataAccessor.UnsetRelogAsync(unsetAccount.AccountId);
                responseList.Add(response);
            }

            var failures = responseList.Where(x => x.Success != true);

            if (failures.Any())
                return StatusRM.CreateError(string.Concat(failures.SelectMany(x => x.ErrorMessage)));

            return StatusRM.CreateSuccess(null, "Successfully unset relog");
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
                    LastUpdated = account.LastUpdated?.DateToCentral(),
                    LastUpdatedDisplayValue = account.LastUpdated?.DateToCentral().ToString("MM/dd/yyyy hh:mm tt"),
                    Name = account.Name,
                    NickName = account.NickName ?? account.Name,
                    ReLoginRequired = account.ReLoginRequired
                })
            };

            return viewModel;
        }

        public async Task<DashboardEditAccountVM> GetIndividualAccountVMAsync(string Id, ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            var dataModel = await _accountDataAccessor.GetAccountDMAsync(userId, Id);

            var account = dataModel.Accounts.FirstOrDefault();
            var publicToken = string.Empty;
            if (account.ReLoginRequired)
            {
                var responseModel = await _plaidDataAccessor.PublicTokenUpdateAsync(account);
                publicToken = responseModel.Link_Token;
            }

            var viewModel = new DashboardEditAccountVM
            {
                AccountId = account.AccountId,
                NickName = account.NickName ?? account.Name,
                AccountTypeId = account.AccountTypeId,
                AvailableBalance = account.AvailableBalance,
                CurrentBalance = account.CurrentBalance,
                CurrentBalanceDisplayValue = account.CurrentBalance?.ToString("c") ?? "$0.00",
                AvailableBalanceDisplayValue = account.AvailableBalance?.ToString("c") ?? "$0.00",
                CurrencyCode = account.CurrencyCode,
                LastUpdated = account.LastUpdated?.DateToCentral().ToString("MM/dd/yyyy hh:mm tt"),
                Mask = account.Mask,
                Name = account.Name,
                UseForBudgeting = account.UseForBudgeting,
                Transactions = account.Transactions.EmptyIfNull().Select(x => new TransactionListVM(x))
                    .OrderByDescending(t => t.TransactionDate),
                RelogRequired = account.ReLoginRequired,
                PublicToken = publicToken
            };

            return viewModel;
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

            var nextContribution = dataModel.Goals
                .Where(goal => goal.ExpenseFlag == isExpense 
                    && !goal.Paused 
                    && goal.NextContributionAmount != 0)
                .Min(goal => goal.NextContributionDate);

            var totalContribution = dataModel.Goals
                .Where(goal => goal.NextContributionDate.Date == nextContribution.Date 
                    && goal.ExpenseFlag == isExpense 
                    && !goal.Paused 
                    && goal.NextContributionAmount != 0)
                .Sum(goal => goal.NextContributionAmount);
            
            var viewModel = new DashboardGoalListVM
            {
                Goals = dataModel.Goals.Where(goal => goal.ExpenseFlag == isExpense)
                .Select(x => new DashboardGoalVM
                {
                    GoalAmount = x.GoalAmount.ToString("c"),
                    UserAccountId = x.UserAccountId,
                    CurrentBalance = (x.AmountContributed - x.AmountSpent).ToString("c"),
                    DesiredCompletionDate = x.DesiredCompletionDate,
                    GoalId = x.GoalId,
                    GoalName = x.GoalName,
                    ExpenseFlag = x.ExpenseFlag,
                    RecurrenceTimeFrame = x.RecurrenceTimeFrame,
                    Paused = x.Paused,
                    ProgressPercent = x.AmountContributed == 0 ? "0%"
                        : (((x.AmountContributed - x.AmountSpent) / x.GoalAmount) * 100).ToString() + "%",
                    NextContributionAmount = x.NextContributionAmount.ToString("c"),
                    NextContributionDate = x.NextContributionDate.DateToCentral().ToString("MM/dd")
                }).OrderBy(g => g.GoalName),
                NextContributionDate = nextContribution.DateToCentral().ToShortDateString(),
                NextContributionTotal = totalContribution.ToString("c")
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
                    ExpenseFlag = isExpense,
                    FundingHistory = new List<DashboardFundingHistoryVM>()
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

            //Get goal history
            viewModel.FundingHistory = new List<DashboardFundingHistoryVM>();

            var fundingHistory = await _budgetingDataAccessor.GetFundingHistoryListDMAsync(goalId);
            viewModel.FundingHistory = fundingHistory.FundingHistories.EmptyIfNull().Select(f => new DashboardFundingHistoryVM
            {
                Amount = f.Amount,
                AutomatedTransfer = f.AutomatedTransfer,
                FromAccount = f.FromAccountName ?? "Ready to Spend",
                ToAccount = f.ToAccountName ?? "Ready to Spend",
                FundingHistoryId = f.FundingHistoryId,
                Note = f.Note,
                TransferDate = f.TransferDate.DateToCentral()
            }).OrderByDescending(f => f.TransferDate.DateToCentral()).ToList();

            return viewModel;
        }

        public async Task<StatusRM> UpdateGoalAsync(DashboardSaveGoalAM actionModel)
        {
            var nextContribution = new ContributionDM();
            var schedules = await _budgetingDataAccessor.GetFundingScheduleListDMAsync(actionModel.UserAccountId);
            var currentData = new GoalDM();

            if (!string.IsNullOrWhiteSpace(actionModel.GoalId))
            {
                currentData = await _budgetingDataAccessor.GetGoalDMAsync(actionModel.GoalId);
            }

            var dataModel = new GoalDM
            {
                GoalName = actionModel.GoalName,
                GoalAmount = actionModel.GoalAmount,
                DesiredCompletionDate = actionModel.DesiredCompletionDate,
                FundingScheduleId = actionModel.FundingScheduleId,
                RecurrenceTimeFrame = actionModel.RecurrenceTimeFrame,
                ExpenseFlag = actionModel.ExpenseFlag,
                Paused = actionModel.Paused,
                AutoSpendMerchantName = actionModel.AutoSpendMerchantName,
                AutoRefill = actionModel.AutoRefill,
                UserAccountId = actionModel.UserAccountId,
                GoalId = actionModel.GoalId,
                AmountContributed = currentData.AmountContributed,
                AmountSpent = currentData.AmountSpent
            };

            foreach (var schedule in schedules.FundingSchedules)
            {
                if (schedule.FundingScheduleId == actionModel.FundingScheduleId)
                {
                    nextContribution = CalculateNextContribution(dataModel, schedule, DateTime.UtcNow);
                }
            }

            dataModel.NextContributionAmount = nextContribution.NextContributionAmount;
            dataModel.NextContributionDate = nextContribution.NextContributionDate;

            return await _budgetingDataAccessor.SaveGoalAsync(dataModel);
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
                AccountName = dataModel.AccountName,
                AccountOwner = dataModel.AccountOwner,
                Address = dataModel.Address,
                Amount = dataModel.Amount.GetValueOrDefault(),
                UserAccountId = dataModel.UserAccountId,
                City = dataModel.City,
                Country = dataModel.Country,
                CurrencyCode = dataModel.CurrencyCode,
                TransactionDate = dataModel.TransactionDate?.DateToCentral(),
                InternalCategory = dataModel.InternalCategory,
                MerchantName = !string.IsNullOrWhiteSpace(dataModel.MerchantName) ? dataModel.MerchantName : "-",
                Name = dataModel.Name,
                Pending = dataModel.Pending,
                TransactionId = dataModel.TransactionId,
            };

            if (goals.Goals.Any())
            {

                viewModel.Goals = goals.Goals.EmptyIfNull().OrderBy(x => x.GoalName).Select(goal => new SelectListItem
                {
                    Text = $"{goal.GoalName} - {(goal.AmountContributed - goal.AmountSpent).ToString("c")}",
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
            var existingTransaction = await _accountDataAccessor.GetTransactionDMAsync(actionModel.TransactionId);
            var toGoal = "0";
            var fromGoal = actionModel.SpendingFromId;

            if (string.IsNullOrWhiteSpace(actionModel.SpendingFromId))
            {
                actionModel.SpendingFromId = null;
                toGoal = existingTransaction.SpendingFrom;
                fromGoal = "0";
            }

            var responseModel = await _accountDataAccessor.SaveTransactionAsync(actionModel);

            if (!string.IsNullOrWhiteSpace(responseModel.ErrorMessage))
            {
                return StatusRM.CreateError(responseModel.ErrorMessage);
            }

            var moveMoneyModel = new DashboardMoveMoneyAM
            {
                Amount = responseModel.Amount ?? 0.00M,
                AutomatedTransfer = false,
                FromAccountId = fromGoal,
                ToAccountId = toGoal,
                UserAccountId = responseModel.UserAccountId,
                Note = "Automated: Spending from transaction",
                TransferDate = DateTime.UtcNow
            };

            return await SaveMoveMoneyAsync(moveMoneyModel);
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
                    FirstContributionDate = schedule.FirstContributionDate.DateToCentral(),
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
                FirstContributionDate = dataModel.FirstContributionDate.DateToCentral(),
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

            viewModel.AccountsList = goalsList.Goals.EmptyIfNull().OrderBy(x => x.GoalName).Select(goal => new SelectListItem
            {
                Text = $"{goal.GoalName} - {(goal.AmountContributed - goal.AmountSpent).ToString("c")}",
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

        public async Task<StatusRM> SaveMoveMoneyAsync(DashboardMoveMoneyAM actionModel, bool autoRefill = true)
        {
            var requestModel = new MoveMoneyRequestModel
            {
                Amount = actionModel.Amount,
                AutomatedTransfer = actionModel.AutomatedTransfer,
                AutoRefill = autoRefill,
                FromAccountId = actionModel.FromAccountId,
                ToAccountId = actionModel.ToAccountId,
                UserAccountId = actionModel.UserAccountId,
                Note = actionModel.Note,
                TransferDate = actionModel.TransferDate
            };

            var responseModel = await _budgetingDataAccessor.SaveMoveMoneyAsync(requestModel);

            if (responseModel.Success)
            {
                if (actionModel.FromAccountId != "0")
                {
                    var fromGoal = await _budgetingDataAccessor.GetGoalDMAsync(actionModel.FromAccountId);
                    var fromSchedule = await _budgetingDataAccessor.GetFundingScheduleDMAsync(fromGoal.FundingScheduleId);

                    //set next contribution to null value to re-calculate
                    fromGoal.NextContributionDate = Convert.ToDateTime("0001-01-01 00:00:00");

                    var nextContribution = CalculateNextContribution(fromGoal, fromSchedule, DateTime.UtcNow);

                    fromGoal.NextContributionAmount = nextContribution.NextContributionAmount;
                    fromGoal.NextContributionDate = nextContribution.NextContributionDate;
                    responseModel = await _budgetingDataAccessor.SaveGoalAsync(fromGoal);
                }

                if (actionModel.ToAccountId != "0")
                {
                    var toGoal = await _budgetingDataAccessor.GetGoalDMAsync(actionModel.ToAccountId);
                    var toSchedule = await _budgetingDataAccessor.GetFundingScheduleDMAsync(toGoal.FundingScheduleId);

                    //set next contribution to null value to re-calculate
                    toGoal.NextContributionDate = Convert.ToDateTime("0001-01-01 00:00:00");

                    var nextContribution = CalculateNextContribution(toGoal, toSchedule, DateTime.UtcNow);

                    toGoal.NextContributionAmount = nextContribution.NextContributionAmount;
                    toGoal.NextContributionDate = nextContribution.NextContributionDate;
                    responseModel = await _budgetingDataAccessor.SaveGoalAsync(toGoal);
                }
            }

            return responseModel;
        }

        public async Task UpdateGoalFunding(string userId, DateTime todayDateTime)
        {
            var goals = await _budgetingDataAccessor.GetGoalListDMAsync(userId);
            var today = todayDateTime.Date;
            var schedules = await _budgetingDataAccessor.GetFundingScheduleListDMAsync(userId);

            //Goal calculation
            foreach (var goal in goals.Goals.Where(g => !g.Paused).ToList())
            {
                if ((goal.DesiredCompletionDate >= today || goal.ExpenseFlag) && goal.NextContributionDate <= today)
                {
                    foreach (var schedule in schedules.FundingSchedules.ToList())
                    {
                        if (schedule.FundingScheduleId == goal.FundingScheduleId)
                        {
                            var nextContributionDate = goal.NextContributionDate;
                            var nextContribution = new ContributionDM();
                            var currentGoal = goal;

                            var fundingHistory = await _budgetingDataAccessor.GetFundingHistoryListDMAsync(currentGoal.GoalId);
                            var goalHistory = fundingHistory.FundingHistories.EmptyIfNull()
                                .Where(g => g.ToAccountId == goal.GoalId && g.AutomatedTransfer == true)
                                .OrderByDescending(g => g.TransferDate)
                                .ToList();

                            var lastFunded = today;
                            if (goalHistory.Any())
                            {
                                lastFunded = goalHistory.Max(g => g.TransferDate);
                            }
                            else
                            {
                                lastFunded = Convert.ToDateTime("0001-01-01 00:00:00");
                            }

                            while (nextContributionDate <= today)
                            {
                                var requestModel = new MoveMoneyRequestModel
                                {
                                    Amount = currentGoal.NextContributionAmount,
                                    AutomatedTransfer = true,
                                    FromAccountId = "0",
                                    ToAccountId = currentGoal.GoalId,
                                    UserAccountId = userId,
                                    Note = "Automatic funding from " + schedule.FundingScheduleName,
                                    TransferDate = currentGoal.NextContributionDate
                                };

                                var response = await _budgetingDataAccessor.SaveMoveMoneyAsync(requestModel);
                                lastFunded = currentGoal.NextContributionDate;

                                currentGoal = await _budgetingDataAccessor.GetGoalDMAsync(currentGoal.GoalId);
                                nextContribution = CalculateNextContribution(currentGoal, schedule, lastFunded);

                                if (!response.Success)
                                    break;


                                currentGoal.NextContributionAmount = nextContribution.NextContributionAmount;
                                currentGoal.NextContributionDate = nextContribution.NextContributionDate;

                                response = await _budgetingDataAccessor.SaveGoalAsync(currentGoal);
                                if (!response.Success)
                                    break;

                                nextContributionDate = nextContribution.NextContributionDate;
                            }
                        }
                    }
                }
            }
        }

        public ContributionDM CalculateNextContribution(GoalDM goal, FundingScheduleDM schedule, DateTime todayDate)
        {
            var nextContributionDate = goal.NextContributionDate;
            var contributionFrequency = schedule.Frequency;
            var numberOfContributionsRemaining = 0;
            var completionDate = goal.DesiredCompletionDate;
            var nextContributionAmount = 0.00M;
            var recurrence = goal.RecurrenceTimeFrame;

            todayDate = todayDate.Date;

            //if new goal, figure out first contribution date
            if (goal.NextContributionDate == Convert.ToDateTime("0001-01-01 00:00:00"))
            {
                var scheduleDate = schedule.FirstContributionDate;
                if (scheduleDate > todayDate)
                {
                    nextContributionDate = schedule.FirstContributionDate;
                }
                else
                {
                    while (todayDate >= scheduleDate)
                        switch (contributionFrequency)
                        {
                            case 1:
                                scheduleDate = scheduleDate.AddDays(7).Date;
                                break;
                            case 2:
                                scheduleDate = scheduleDate.AddDays(14).Date;
                                break;
                            case 3:
                                scheduleDate = scheduleDate.AddMonths(1).Date;
                                break;
                            case 4:
                                scheduleDate = scheduleDate.AddMonths(2).Date;
                                break;
                                //to do
                                //case 5:
                                //    var newMonth = nextContribution.AddDays(1);
                                //    nextContribution = new DateTime(newMonth.Day,
                                //        newMonth.Month,
                                //        DateTime.DaysInMonth(newMonth.AddDays(1).Year, newMonth.Month));
                                //break;
                        }
                }

                nextContributionDate = scheduleDate;
            }
            else
            {
                switch (contributionFrequency)
                {
                    case 1:
                        nextContributionDate = nextContributionDate.AddDays(7).Date;
                        break;
                    case 2:
                        nextContributionDate = nextContributionDate.AddDays(14).Date;
                        break;
                    case 3:
                        nextContributionDate = nextContributionDate.AddMonths(1).Date;
                        break;
                    case 4:
                        nextContributionDate = nextContributionDate.AddMonths(2).Date;
                        break;
                        //to do
                        //case 5:
                        //    var newMonth = nextContribution.AddDays(1);
                        //    nextContribution = new DateTime(newMonth.Day,
                        //        newMonth.Month,
                        //        DateTime.DaysInMonth(newMonth.AddDays(1).Year, newMonth.Month));
                        //break;
                }
            }

            if (goal.ExpenseFlag)
            {
                if (completionDate <= todayDate)
                {

                    if (recurrence == 1)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddDays(7);
                        }
                    }
                    else if (recurrence == 2)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddDays(14);
                        }
                    }
                    else if (recurrence == 3)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddMonths(1);
                        }
                    }
                    else if (recurrence == 4)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddMonths(2);
                        }
                    }
                    else if (recurrence == 5)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddMonths(3);
                        }
                    }
                    else if (recurrence == 6)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddMonths(6);
                        }
                    }
                    else if (recurrence == 7)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddYears(1);
                        }
                    }
                }
            }

            var counterDate = nextContributionDate;

            switch (contributionFrequency)
            {
                case 1:
                    while (completionDate >= counterDate)
                    {
                        counterDate = counterDate.AddDays(7);
                        numberOfContributionsRemaining++;
                    }
                    break;
                case 2:
                    while (completionDate >= counterDate)
                    {
                        counterDate = counterDate.AddDays(14);
                        numberOfContributionsRemaining++;
                    }
                    break;
                case 3:
                    while (completionDate >= counterDate)
                    {
                        counterDate = counterDate.AddMonths(1);
                        numberOfContributionsRemaining++;
                    }
                    break;
                case 4:
                    while (completionDate >= counterDate)
                    {
                        counterDate = counterDate.AddMonths(2);
                        numberOfContributionsRemaining++;
                    }
                    break;
            }

            if (numberOfContributionsRemaining > 0)
            {
                nextContributionAmount = Math.Round((goal.GoalAmount - goal.AmountContributed) / numberOfContributionsRemaining, 2);
                if (nextContributionAmount < 0)
                {
                    nextContributionAmount = 0;
                }
            }
            else
            {
                nextContributionAmount = 0;
            }

            var dataModel = new ContributionDM
            {
                NextContributionDate = nextContributionDate,
                NextContributionAmount = nextContributionAmount
            };

            return dataModel;
        }
    }
}