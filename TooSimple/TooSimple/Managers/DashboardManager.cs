using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.Data;
using TooSimple.Models.ViewModels;

namespace TooSimple.Managers
{
    public class DashboardManager : IDashboardManager
    {
        private ApplicationDbContext _db;
        public DashboardManager(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardVM> GetDashboardVMAsync(ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            var viewModel = _db.Accounts.Select(a => new DashboardVM
            {
                AccessToken = a.AccessToken,
                AccountId = a.AccountId,
                AccountTypeId = a.AccountTypeId,
                AvailableBalance = a.AvailableBalance,
                PlaidAccountId = a.PlaidAccountId,
                CurrencyCode = a.CurrencyCode,
                CurrentBalance = a.CurrentBalance,
                Mask = a.Mask,
                Name = a.Name,
                NickName = a.NickName,
                Transactions = a.Transactions.Select(x => new TransactionListVM
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
                })
            });

            
            var viewModel = new DashboardVM
            {
                AccessToken = account.Select(x => x.AccessToken).ToString(),
                
            };

            return viewModel;
        }
    }
}
