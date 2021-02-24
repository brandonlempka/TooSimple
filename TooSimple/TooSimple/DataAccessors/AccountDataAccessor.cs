using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Data;
using TooSimple.Extensions;
using TooSimple.Models.ActionModels;
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

        public async Task<AccountListDM> GetAccountDMAsync(string userId, string accountId = "")
        {
            var data = await _db.Accounts.Where(a => a.UserAccountId == userId).ToListAsync();

            if (data == null)
            {
                return new AccountListDM();
            }

            //If accountId is supplied, return just the requested account
            if (!string.IsNullOrWhiteSpace(accountId))
            {
                var singleAccount = new AccountListDM
                {
                    Accounts = data.Where(x => x.AccountId == accountId)
                                   .Select(x => new AccountDM
                                   {
                                       AccessToken = x.AccessToken,
                                       AccountId = x.AccountId,
                                       AccountTypeId = x.AccountTypeId,
                                       AvailableBalance = x.AvailableBalance,
                                       UserAccountId = x.UserAccountId,
                                       CurrencyCode = x.CurrencyCode,
                                       CurrentBalance = x.CurrentBalance,
                                       Mask = x.Mask,
                                       Name = x.Name,
                                       NickName = x.NickName,
                                       LastUpdated = x.LastUpdated,
                                       UseForBudgeting = x.UseForBudgeting,
                                       Transactions = from tran in _db.Transactions
                                                      where tran.AccountId == accountId
                                                      select new TransactionDM
                                                      {
                                                          AccountId = tran.AccountId,
                                                          AccountOwner = tran.AccountOwner,
                                                          Address = tran.Address,
                                                          Amount = tran.Amount,
                                                          UserAccountId = tran.UserAccountId,
                                                          City = tran.City,
                                                          Country = tran.Country,
                                                          CurrencyCode = tran.CurrencyCode,
                                                          InternalCategory = tran.InternalCategory,
                                                          MerchantName = tran.MerchantName,
                                                          Name = tran.Name,
                                                          PaymentMethod = tran.PaymentMethod,
                                                          Pending = tran.Pending,
                                                          PostalCode = tran.PostalCode,
                                                          Region = tran.Region,
                                                          SpendingFrom = tran.SpendingFrom,
                                                          TransactionCode = tran.TransactionCode,
                                                          TransactionDate = tran.TransactionDate,
                                                          TransactionId = tran.TransactionId,
                                                      }
                                   })
                };

                return singleAccount;
            }

            var allAccounts = new AccountListDM
            {
                Accounts = data.Select(x => new AccountDM
                {
                    AccessToken = x.AccessToken,
                    AccountId = x.AccountId,
                    AccountTypeId = x.AccountTypeId,
                    AvailableBalance = x.AvailableBalance,
                    UserAccountId = x.UserAccountId,
                    CurrencyCode = x.CurrencyCode,
                    CurrentBalance = x.CurrentBalance,
                    Mask = x.Mask,
                    Name = x.Name,
                    NickName = x.NickName,
                    LastUpdated = x.LastUpdated,
                    UseForBudgeting = x.UseForBudgeting,
                    Transactions = from tran in _db.Transactions
                                   where tran.UserAccountId == userId
                                   select new TransactionDM
                                   {
                                       AccountId = tran.AccountId,
                                       AccountOwner = tran.AccountOwner,
                                       Address = tran.Address,
                                       Amount = tran.Amount,
                                       UserAccountId = tran.UserAccountId,
                                       City = tran.City,
                                       Country = tran.Country,
                                       CurrencyCode = tran.CurrencyCode,
                                       InternalCategory = tran.InternalCategory,
                                       MerchantName = tran.MerchantName,
                                       Name = tran.Name,
                                       PaymentMethod = tran.PaymentMethod,
                                       Pending = tran.Pending,
                                       PostalCode = tran.PostalCode,
                                       Region = tran.Region,
                                       SpendingFrom = tran.SpendingFrom,
                                       TransactionCode = tran.TransactionCode,
                                       TransactionDate = tran.TransactionDate,
                                       TransactionId = tran.TransactionId,
                                   }
                })
            };

            return allAccounts;
        }

        public async Task<StatusRM> SavePlaidAccountData(IEnumerable<AccountDM> dataModel)
        {
            try
            {
                foreach (var account in dataModel)
                {
                    var existingAccount = await _db.Accounts.FirstOrDefaultAsync(x => x.AccountId == account.AccountId);

                    if (existingAccount == null)
                    {

                        await _db.Accounts.AddAsync(new Account
                        {
                            AccessToken = account.AccessToken,
                            AvailableBalance = account.AvailableBalance,
                            UserAccountId = account.UserAccountId,
                            CurrencyCode = account.CurrencyCode,
                            CurrentBalance = account.CurrentBalance,
                            Mask = account.Mask,
                            Name = account.Name,
                            AccountId = account.AccountId,
                            LastUpdated = DateTime.Now,
                            UseForBudgeting = account.UseForBudgeting
                        });

                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        existingAccount.CurrentBalance = account.CurrentBalance;
                        existingAccount.AvailableBalance = account.AvailableBalance;
                        existingAccount.LastUpdated = DateTime.Now;
                        existingAccount.UseForBudgeting = account.UseForBudgeting;

                        await _db.SaveChangesAsync();
                    }
                }

                return StatusRM.CreateSuccess(null, "Successfully updated account.");

            }
            catch (Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }

        public async Task<StatusRM> SavePlaidTransactionData(IEnumerable<TransactionDM> dataModel)
        {
            try
            {
                foreach (var transaction in dataModel)
                {
                    var existingTransaction = await _db.Transactions.FirstOrDefaultAsync(t => t.TransactionId == transaction.TransactionId);

                    if (existingTransaction == null)
                    {
                        await _db.Transactions.AddAsync(new Transaction
                        {
                            TransactionId = transaction.TransactionId,
                                            AccountId = transaction.AccountId,
                                            AccountOwner = transaction.AccountOwner,
                                            Amount = transaction.Amount ?? 0,
                                            TransactionDate = transaction.TransactionDate,
                                            CurrencyCode = transaction.CurrencyCode,
                                            Address = transaction.Address,
                                            City = transaction.City,
                                            Country = transaction.Country,
                                            PostalCode = transaction.PostalCode,
                                            Region = transaction.Region,
                                            MerchantName = transaction.MerchantName,
                                            Name = transaction.Name,
                                            PaymentMethod = transaction.PaymentMethod,
                                            ReferenceNumber = transaction.ReferenceNumber,
                                            Pending = transaction.Pending,
                                            TransactionCode = transaction.TransactionCode,
                                            UnofficialCurrencyCode = transaction.CurrencyCode,
                                            SpendingFrom = transaction.SpendingFrom,
                                            InternalCategory = transaction.InternalCategory,
                                            UserAccountId = transaction.UserAccountId
                        });
                    }
                }

                await _db.SaveChangesAsync();
                return StatusRM.CreateSuccess(null, "Success");

                //foreach (var transaction in dataModel)
                //{
                //    var sql = @"MERGE INTO [dbo].[Transaction]
                //                USING 
                //                (
                //                    SELECT   @TransactionId as Id
                            
                //                ) AS entity
                //                ON  [dbo].[Transaction].[TransactionId] = entity.Id
                //                WHEN NOT MATCHED THEN
                //                    INSERT (TransactionId
                //                        , AccountId
                //                        , AccountOwner
                //                        , Amount
                //                        , TransactionDate
                //                        , CurrencyCode
                //                        , Address
                //                        , City
                //                        , Country
                //                        , PostalCode
                //                        , Region
                //                        , MerchantName
                //                        , Name
                //                        , PaymentMethod
                //                        , ReferenceNumber
                //                        , Pending
                //                        , TransactionCode
                //                        , UnofficialCurrencyCode
                //                        , SpendingFrom
                //                        , InternalCategory
                //                        , UserAccountId)
                //                    VALUES (@TransactionId
                //                        , @AccountId
                //                        , @AccountOwner
                //                        , @Amount
                //                        , @TransactionDate
                //                        , @CurrencyCode
                //                        , @Address
                //                        , @City
                //                        , @Country
                //                        , @PostalCode
                //                        , @Region
                //                        , @MerchantName
                //                        , @Name
                //                        , @PaymentMethod
                //                        , @ReferenceNumber
                //                        , @Pending
                //                        , @TransactionCode
                //                        , @UnofficialCurrencyCode
                //                        , @SpendingFrom
                //                        , @InternalCategory
                //                        , @UserAccountId);";

                //    object[] parameters =
                //    {
                //        new SqlParameter("@TransactionId", DBExtensions.DBValue(transaction.TransactionId)),
                //        new SqlParameter("@AccountId", DBExtensions.DBValue(transaction.AccountId)),
                //        new SqlParameter("@AccountOwner", DBExtensions.DBValue(transaction.AccountOwner)),
                //        new SqlParameter("@Amount", DBExtensions.DBValue(transaction.Amount)),
                //        new SqlParameter("@TransactionDate", DBExtensions.DBValue(transaction.TransactionDate)),
                //        new SqlParameter("@CurrencyCode", DBExtensions.DBValue(transaction.CurrencyCode)),
                //        new SqlParameter("@Address", DBExtensions.DBValue(transaction.Address)),
                //        new SqlParameter("@City", DBExtensions.DBValue(transaction.City)),
                //        new SqlParameter("@Country", DBExtensions.DBValue(transaction.Country)),
                //        new SqlParameter("@PostalCode", DBExtensions.DBValue(transaction.PostalCode)),
                //        new SqlParameter("@Region", DBExtensions.DBValue(transaction.Region)),
                //        new SqlParameter("@MerchantName", DBExtensions.DBValue(transaction.MerchantName)),
                //        new SqlParameter("@Name", DBExtensions.DBValue(transaction.Name)),
                //        new SqlParameter("@PaymentMethod", DBExtensions.DBValue(transaction.PaymentMethod)),
                //        new SqlParameter("@ReferenceNumber", DBExtensions.DBValue(transaction.ReferenceNumber)),
                //        new SqlParameter("@Pending", DBExtensions.DBValue(transaction.Pending)),
                //        new SqlParameter("@TransactionCode", DBExtensions.DBValue(transaction.TransactionCode)),
                //        new SqlParameter("@UnofficialCurrencyCode", DBExtensions.DBValue(transaction.CurrencyCode)),
                //        new SqlParameter("@SpendingFrom", DBExtensions.DBValue(transaction.SpendingFrom)),
                //        new SqlParameter("@InternalCategory", DBExtensions.DBValue(transaction.InternalCategory)),
                //        new SqlParameter("@UserAccountId", DBExtensions.DBValue(transaction.UserAccountId)),
                //    };

                //    await _db.Database.ExecuteSqlRawAsync(sql, parameters);
                //}

                ////await context.SaveChangesAsync();
                //return StatusRM.CreateSuccess(null, "Successfully updated transactions.");
            }
            catch (Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }

        public async Task<StatusRM> UpdateAccountAsync(DashboardSaveAccountAM actionModel)
        {
            try
            {
                var account = await _db.Accounts.FirstOrDefaultAsync(x => x.AccountId == actionModel.AccountId);

                account.NickName = actionModel.NickName;
                account.UseForBudgeting = actionModel.UseForBudgeting;
                await _db.SaveChangesAsync();
                return StatusRM.CreateSuccess("Index", "Successfully updated your account.");
            }
            catch (Exception ex)
            {
                return StatusRM.CreateError(ex);
            }

        }

        public async Task<StatusRM> DeleteAccountAsync(string accountId)
        {
            try
            {
                var transactions = _db.Transactions
                    .Where(tran => tran.AccountId == accountId);

                _db.Transactions.RemoveRange(transactions);

                var account = new Account
                {
                    AccountId = accountId
                };

                _db.Accounts.Attach(account);
                _db.Accounts.Remove(account);
                await _db.SaveChangesAsync();

                return StatusRM.CreateSuccess("Accounts", "Successfully deleted account.");
            }
            catch (Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }

        public async Task<TransactionListDM> GetSpendingFromTransactions(string userId)
        {
            var dataModel = new TransactionListDM
            {
                Transactions = from tran in _db.Transactions
                               join account in _db.Accounts on tran.AccountId equals account.AccountId
                               where !string.IsNullOrWhiteSpace(tran.SpendingFrom)
                               select new TransactionDM
                               {
                                   AccountId = tran.AccountId,
                                   AccountOwner = tran.AccountOwner,
                                   Address = tran.Address,
                                   Amount = tran.Amount,
                                   UserAccountId = tran.UserAccountId,
                                   City = tran.City,
                                   Country = tran.Country,
                                   CurrencyCode = tran.CurrencyCode,
                                   InternalCategory = tran.InternalCategory,
                                   MerchantName = tran.MerchantName,
                                   Name = tran.Name,
                                   PaymentMethod = tran.PaymentMethod,
                                   Pending = tran.Pending,
                                   PostalCode = tran.PostalCode,
                                   ReferenceNumber = tran.ReferenceNumber,
                                   Region = tran.Region,
                                   SpendingFrom = tran.SpendingFrom,
                                   TransactionCode = tran.TransactionCode,
                                   TransactionDate = tran.TransactionDate,
                                   TransactionId = tran.TransactionId
                               }
            };

            return dataModel;
        }

        public async Task<TransactionDM> GetTransactionDMAsync(string transactionId)
        {
            var transaction = await _db.Transactions.FirstOrDefaultAsync(tran => tran.TransactionId == transactionId);

            if (transaction == null)
                return new TransactionDM();

            return new TransactionDM
            {

                AccountId = transaction.AccountId,
                AccountOwner = transaction.AccountOwner,
                Address = transaction.Address,
                Amount = transaction.Amount,
                UserAccountId = transaction.UserAccountId,
                City = transaction.City,
                Country = transaction.Country,
                CurrencyCode = transaction.CurrencyCode,
                InternalCategory = transaction.InternalCategory,
                MerchantName = transaction.MerchantName,
                Name = transaction.Name,
                PaymentMethod = transaction.PaymentMethod,
                Pending = transaction.Pending,
                PostalCode = transaction.PostalCode,
                ReferenceNumber = transaction.ReferenceNumber,
                Region = transaction.Region,
                SpendingFrom = transaction.SpendingFrom,
                TransactionCode = transaction.TransactionCode,
                TransactionDate = transaction.TransactionDate,
                TransactionId = transaction.TransactionId
            };
        }

        public async Task<StatusRM> SaveTransactionAsync(DashboardEditTransactionAM actionModel)
        {
            try
            {
                var existingTran = await _db.Transactions.FirstOrDefaultAsync(tran => tran.TransactionId == actionModel.TransactionId);

                existingTran.SpendingFrom = actionModel.SpendingFromId;
                existingTran.InternalCategory = actionModel.InternalCategory;

                await _db.SaveChangesAsync();

                return StatusRM.CreateSuccess(null, "Success");
            }
            catch (Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }
    }
}