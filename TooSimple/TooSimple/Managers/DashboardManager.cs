using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.Data;
using TooSimple.DataAccessors;
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

        public DashboardManager(IAccountDataAccessor accountDataAccessor, IPlaidDataAccessor plaidDataAccessor)
        {
            _accountDataAccessor = accountDataAccessor;
            _plaidDataAccessor = plaidDataAccessor;
        }

        public async Task<DashboardVM> GetDashboardVMAsync(ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            var dataModel = await _accountDataAccessor.GetAccountDM(userId);

            if (dataModel.AccessToken == null)
            {
                var linkToken = await _plaidDataAccessor.CreateLinkTokenAsync(userId);
                var emptyViewModel = new DashboardVM
                {
                    LinkToken = linkToken.Link_Token
                };

                return emptyViewModel;
            }

            var viewModel = new DashboardVM
            {
                AccessToken = dataModel.AccessToken,
                AccountId = dataModel.AccountId,
                AccountTypeId = dataModel.AccountTypeId,
                AvailableBalance = dataModel.AvailableBalance,
                PlaidAccountId = dataModel.PlaidAccountId,
                CurrencyCode = dataModel.CurrencyCode,
                CurrentBalance = dataModel.CurrentBalance,
                Mask = dataModel.Mask,
                Name = dataModel.Name,
                NickName = dataModel.NickName
            };

            var transactionsRequest = new PlaidTransactionRequestModel
            {
                AccessToken = dataModel.AccessToken,
                StartDate = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd"),
            };

            var transactionDataModel = await _plaidDataAccessor.GetTransactionsAsync(transactionsRequest);

            if (transactionDataModel.transactions.Any())
            {
                viewModel.Transactions = transactionDataModel.transactions.Select(x => new TransactionListVM
                {
                    AccountOwner = x.account_owner,
                    Address = x.location.address,
                    Amount = x.amount,
                    PlaidAccountId = x.account_id,
                    City = x.location.city,
                    Country = x.location.country,
                    CurrencyCode = x.iso_currency_code,
                    MerchantName = x.merchant_name,
                    Name = x.name,
                    PaymentMethod = x.payment_channel,
                    Pending = x.pending,
                    PlaidTransactionId = x.transaction_id,
                    PostalCode = x.location.postal_code,
                    Region = x.location.region,
                    TransactionCode = x.transaction_code.ToString(),
                    TransactionDate = Convert.ToDateTime(x.date),
                });
            }

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

            var responseModel = await _plaidDataAccessor.PublicTokenExchangeAsync(dataModel.public_token);

            if (string.IsNullOrWhiteSpace(responseModel.Access_token))
            {
                return StatusRM.CreateError(genericError);
            }

            var account = await _plaidDataAccessor.GetAccountBalancesAsync(responseModel.Access_token);

            if (account == null)
            {
                return StatusRM.CreateError(genericError);
            }

            var newAccount = account.accounts.Select(x => new AccountDM
            {
                AccessToken = responseModel.Access_token,
                UserAccountId = userId,
                AvailableBalance = x.balances.available,
                CurrentBalance = x.balances.current,
                PlaidAccountId = x.account_id,
                CurrencyCode = x.balances.iso_currency_code,
                Mask = x.mask,
                Name = x.name,
            });

            var accountAddResponse = await _accountDataAccessor.SavePlaidAccountData(newAccount);

            var transactionsRequest = new PlaidTransactionRequestModel
            {
                AccessToken = responseModel.Access_token,
                StartDate = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd"),
            };

            var transactions = await _plaidDataAccessor.GetTransactionsAsync(transactionsRequest);

            var newTransactions = transactions.transactions.Select(x => new TransactionDM
            {
                AccountOwner = x.account_owner,
                Address = x.location.address,
                Amount = x.amount,
                PlaidAccountId = x.account_id,
                UserAccountId = userId,
                City = x.location.city,
                Country = x.location.country,
                CurrencyCode = x.iso_currency_code,
                MerchantName = x.merchant_name,
                Name = x.name,
                Pending = x.pending,
                PlaidTransactionId = x.transaction_id,
                PostalCode = x.location.postal_code,
                Region = x.location.region,
                TransactionDate = Convert.ToDateTime(x.date),
                TransactionCode = x.transaction_code.ToString()
            });

            return accountAddResponse;
        }

        private async Task<StatusRM> UpdateAccountDbAsync(string userId, string accessToken)
        {

        }

    }
}
