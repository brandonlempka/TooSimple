﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.Data;
using TooSimple.DataAccessors;
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
                var linkToken = await _plaidDataAccessor.CreateLinkTokenAsync(userId);
                var emptyViewModel = new DashboardVM
                {
                    LinkToken = linkToken.Link_Token
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
                var transaction = account.Transactions.Select(x => new TransactionListVM
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

            if (transactionsAddResponse.Success == true && accountAddResponse.Success == true)
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

        public async Task<DashboardGoalsVM> GetGoalsVMAsync(ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var dataModel = await _budgetingDataAccessor.GetGoalListDMAsync(userId);

            return new DashboardGoalsVM
            {
                Goals = dataModel.Goals.Select(x => new DashboardGoalsListVM
                {
                    GoalAmount = x.GoalAmount,
                    UserAccountId = x.UserAccountId,
                    CurrentBalance = x.CurrentBalance,
                    DesiredCompletionDate = x.DesiredCompletionDate,
                    GoalId = x.GoalId,
                    GoalName = x.GoalName
                })
            };
        }

        public async Task<DashboardSaveGoalVM> GetSaveGoalVMAsync(string goalId, ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (goalId == "")
                return new DashboardSaveGoalVM
                {
                    UserAccountId = userId
                };

            var existingAccount = await _budgetingDataAccessor.GetGoalDMAsync(goalId);

            if (existingAccount == null)
                return new DashboardSaveGoalVM
                {
                    UserAccountId = userId
                };

            return new DashboardSaveGoalVM
            {
                GoalAmount = existingAccount.GoalAmount,
                CurrentBalance = existingAccount.CurrentBalance,
                DesiredCompletionDate = existingAccount.DesiredCompletionDate,
                GoalId = existingAccount.GoalId,
                GoalName = existingAccount.GoalName,
                UserAccountId = userId
            };
        }

        public async Task<StatusRM> UpdateGoalAsync(DashboardSaveGoalAM actionModel)
        {
            return await _budgetingDataAccessor.SaveGoalAsync(actionModel);
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
                    Value = "",
                });

            }
            else
            {
                viewModel.Goals.Add(new SelectListItem
                {
                    Text = "Ready to Spend",
                    Value = "",
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

        public async Task<DashboardFundingScheduleListVM> GetDashboardFundingScheduleListVM(string userId)
        {
            var dataModel = await _budgetingDataAccessor.GetFundingScheduleListDMAsync(userId);

            return new DashboardFundingScheduleListVM
            {
                FundingSchedules = dataModel.FundingSchedules.Select(schedule => new FundingScheduleVM
                {
                    UserAccountId = schedule.UserAccountId,
                    FirstContributionDate = schedule.FirstContributionDate,
                    Frequency = schedule.Frequency,
                    FundingScheduleId = schedule.FundingScheduleId,
                    FundingScheduleName = schedule.FundingScheduleName
                })
            };
        }

        public async Task<DashboardFundingScheduleVM> GetDashboardFundingScheduleVM(string scheduleId)
        {
            var dataModel = await _budgetingDataAccessor.GetFundingScheduleDMAsync(scheduleId);

            return new DashboardFundingScheduleVM
            {
                UserAccountId = dataModel.UserAccountId,
                FirstContributionDate = dataModel.FirstContributionDate,
                Frequency = dataModel.Frequency,
                FundingScheduleId = dataModel.FundingScheduleId,
                FundingScheduleName = dataModel.FundingScheduleName
            };
        }

        public async Task<StatusRM> UpdateFundingScheduleAsync(DashboardSaveFundingScheduleAM actionModel)
        {
            return await _budgetingDataAccessor.SaveScheduleAsync(actionModel);
        }
    }
}
