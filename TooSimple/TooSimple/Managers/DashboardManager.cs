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

            var updateResponse = await UpdateAccountDbAsync(userId, dataModel.AccessToken);

            var viewModel = new DashboardVM
            {
                AccessToken = dataModel.AccessToken,
                AccountId = dataModel.AccountId,
                AccountTypeId = dataModel.AccountTypeId,
                AvailableBalance = dataModel.AvailableBalance,
                CurrencyCode = dataModel.CurrencyCode,
                CurrentBalance = dataModel.CurrentBalance,
                Mask = dataModel.Mask,
                Name = dataModel.Name,
                NickName = dataModel.NickName
            };

            if (!updateResponse.Success)
                viewModel.ErrorMessage = "Something went wrong while refreshing your accounts";

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
            var account = await _plaidDataAccessor.GetAccountBalancesAsync(accessToken);

            if (account == null)
            {
                return StatusRM.CreateError(genericError);
            }

            var newAccount = account.accounts.Select(x => new AccountDM
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

            var accountAddResponse = await _accountDataAccessor.SavePlaidAccountData(newAccount);

            if (!string.IsNullOrWhiteSpace(accountAddResponse.ErrorMessage))
            {
                return StatusRM.CreateError(genericError);
            }

            var transactionsRequest = new PlaidTransactionRequestModel
            {
                AccessToken = accessToken,
                StartDate = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd"),
            };

            var transactions = await _plaidDataAccessor.GetTransactionsAsync(transactionsRequest);
            var transactionsAddResponse = new StatusRM();

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
    }
}
