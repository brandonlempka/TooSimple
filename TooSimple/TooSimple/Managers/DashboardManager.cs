using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.Data;
using TooSimple.DataAccessors;
using TooSimple.Models.DataModels;
using TooSimple.Models.DataModels.Plaid;
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

            if (dataModel.Transactions.Any())
            {
                viewModel.Transactions = dataModel.Transactions.Select(x => new TransactionListVM
                {
                    AccountOwner = x.AccountOwner,
                    Address = x.Address,
                    Amount = x.Amount,
                    PlaidAccountId = x.PlaidAccountId,
                    City = x.City,
                    Country = x.Country,
                    CurrencyCode = x.CurrencyCode,
                    InternalCategory = x.InternalCategory,
                    MerchantName = x.MerchantName,
                    Name = x.Name,
                    PaymentMethod = x.PaymentMethod,
                    Pending = x.Pending,
                    PlaidTransactionId = x.PlaidTransactionId,
                    PostalCode = x.PostalCode,
                    Region = x.Region,
                    SpendingFrom = x.SpendingFrom,
                    TransactionCode = x.TransactionCode,
                    TransactionDate = x.TransactionDate,
                    TransactionId = x.TransactionId
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

            var account = await _plaidDataAccessor.AddNewAccountAsync(responseModel.Access_token);

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

            return accountAddResponse;
        }
    }
}
