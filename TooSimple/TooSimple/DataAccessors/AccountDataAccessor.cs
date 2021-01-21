using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Data;
using TooSimple.Models.DataModels;
using TooSimple.Models.DataModels.Plaid;
using TooSimple.Models.EFModels;
using TooSimple.Models.ResponseModels;
using TooSimple.Models.ViewModels;

namespace TooSimple.DataAccessors
{
    public class AccountDataAccessor : IAccountDataAccessor
    {
        private ApplicationDbContext _db;

        public AccountDataAccessor(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<AccountDM> GetAccountDM(string accountId)
        {
            var data = await _db.Accounts.FirstOrDefaultAsync(a => a.UserAccountId == accountId);

            if (data == null)
            {
                return new AccountDM
                {
                    Transactions = Enumerable.Empty<TransactionListDM>()
                };
            }

            using (var context = _db)
            {
                var transactionData = from tran in context.Transactions
                                      where tran.UserAccountId == data.UserAccountId
                                      select new TransactionListDM
                                      {
                                          AccountOwner = tran.AccountOwner,
                                          Address = tran.Address,
                                          Amount = tran.Amount,
                                          PlaidAccountId = tran.PlaidAccountId,
                                          City = tran.City,
                                          Country = tran.Country,
                                          CurrencyCode = tran.CurrencyCode,
                                          InternalCategory = tran.InternalCategory,
                                          MerchantName = tran.MerchantName,
                                          Name = tran.Name,
                                          PaymentMethod = tran.PaymentMethod,
                                          Pending = tran.Pending,
                                          PlaidTransactionId = tran.PlaidTransactionId,
                                          PostalCode = tran.PostalCode,
                                          Region = tran.Region,
                                          SpendingFrom = tran.SpendingFrom,
                                          TransactionCode = tran.TransactionCode,
                                          TransactionDate = tran.TransactionDate,
                                          TransactionId = tran.TransactionId
                                      };

                var dataModel = new AccountDM
                {
                    AccessToken = data.AccessToken,
                    AccountId = data.AccountId,
                    AccountTypeId = data.AccountTypeId,
                    AvailableBalance = data.AvailableBalance,
                    PlaidAccountId = data.PlaidAccountId,
                    CurrencyCode = data.CurrencyCode,
                    CurrentBalance = data.CurrentBalance,
                    Mask = data.Mask,
                    Name = data.Name,
                    NickName = data.NickName,
                    Transactions = transactionData.ToList(),
                };

                return dataModel;
            }
        }

        public async Task<StatusRM> SavePlaidAccountData(IEnumerable<AccountDM> dataModel)
        {
            try
            {
                using (var context = _db)
                {
                    foreach (var account in dataModel)
                    {
                        var existingAccount = context.Accounts.FirstOrDefault(x => x.PlaidAccountId == account.PlaidAccountId);

                        if (existingAccount == null)
                        {
                            foreach (var newAccount in dataModel)
                            {
                                await context.Accounts.AddAsync(new Account
                                {
                                    AccessToken = newAccount.AccessToken,
                                    AvailableBalance = newAccount.AvailableBalance,
                                    PlaidAccountId = newAccount.PlaidAccountId,
                                    UserAccountId = newAccount.UserAccountId,
                                    CurrencyCode = newAccount.CurrencyCode,
                                    CurrentBalance = newAccount.CurrentBalance,
                                    Mask = newAccount.Mask,
                                    Name = newAccount.Name,
                                    NickName = newAccount.NickName,
                                });
                            }

                            await context.SaveChangesAsync();
                        }

                        existingAccount.CurrentBalance = account.CurrentBalance;
                        existingAccount.AvailableBalance = account.AvailableBalance;
                        existingAccount.NickName = account.NickName;
                    }

                    await context.SaveChangesAsync();
                    return StatusRM.CreateSuccess(null, "Successfully updated account.");
                }
            }
            catch(Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }

        public async Task<StatusRM> SavePlaidTransactionData(IEnumerable<TransactionDM> dataModel)
        {
            try
            {
                using (var context = _db)
                {
                    foreach (var transaction in dataModel)
                    {
                        var sql = @"MERGE INTO Transactions
                                    USING 
                                    (
                                       SELECT   @PlaidTransactionId as Id
                            
                                    ) AS entity
                                    ON  Transaction.PlaidTransactionId = entity.Id
                                    WHEN NOT MATCHED THEN
                                        INSERT (PlaidTransactionId)
                                        VALUES (@PlaidTransactionId)";

                        object[] parameters =
                        {
                            new SqlParameter("@PlaidTransactionId", transaction.PlaidTransactionId)
                        };

                        await context.Database.ExecuteSqlRawAsync(sql, parameters);
                    }

                    //await context.SaveChangesAsync();
                    return StatusRM.CreateSuccess(null, "Successfully updated transactions.");
                }
            }
            catch(Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }
    }
}