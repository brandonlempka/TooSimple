using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Data;
using TooSimple.Extensions;
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
            var data = await _db.Accounts.Where(a => a.UserAccountId == accountId).ToListAsync();
            
            if (data == null)
            {
                return new AccountDM();
            }

            var transactionData = from tran in _db.Transactions
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
                CurrencyCode = data.CurrencyCode,
                CurrentBalance = data.CurrentBalance,
                Mask = data.Mask,
                Name = data.Name,
                NickName = data.NickName,
            };

            return dataModel;

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
                        foreach (var newAccount in dataModel)
                        {
                            await _db.Accounts.AddAsync(new Account
                            {
                                AccessToken = newAccount.AccessToken,
                                AvailableBalance = newAccount.AvailableBalance,
                                UserAccountId = newAccount.UserAccountId,
                                CurrencyCode = newAccount.CurrencyCode,
                                CurrentBalance = newAccount.CurrentBalance,
                                Mask = newAccount.Mask,
                                Name = newAccount.Name,
                                NickName = newAccount.NickName,
                                AccountId = newAccount.AccountId
                            });
                        }

                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        existingAccount.CurrentBalance = account.CurrentBalance;
                        existingAccount.AvailableBalance = account.AvailableBalance;
                        existingAccount.NickName = account.NickName;
                        
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
                    var sql = @"MERGE INTO [SQL-TooSimple].[dbo].[Transaction]
                                USING 
                                (
                                    SELECT   @TransactionId as Id
                            
                                ) AS entity
                                ON  [SQL-TooSimple].[dbo].[Transaction].[TransactionId] = entity.Id
                                WHEN NOT MATCHED THEN
                                    INSERT (TransactionId
                                        , PlaidAccountId
                                        , AccountOwner
                                        , Amount
                                        , TransactionDate
                                        , CurrencyCode
                                        , Address
                                        , City
                                        , Country
                                        , PostalCode
                                        , Region
                                        , MerchantName
                                        , Name
                                        , PaymentMethod
                                        , ReferenceNumber
                                        , Pending
                                        , TransactionCode
                                        , UnofficialCurrencyCode
                                        , SpendingFrom
                                        , InternalCategory
                                        , AccountId
                                        , UserAccountId)
                                    VALUES (@TransactionId
                                        , @PlaidAccountId
                                        , @AccountOwner
                                        , @Amount
                                        , @TransactionDate
                                        , @CurrencyCode
                                        , @Address
                                        , @City
                                        , @Country
                                        , @PostalCode
                                        , @Region
                                        , @MerchantName
                                        , @Name
                                        , @PaymentMethod
                                        , @ReferenceNumber
                                        , @Pending
                                        , @TransactionCode
                                        , @UnofficialCurrencyCode
                                        , @SpendingFrom
                                        , @InternalCategory
                                        , @AccountId
                                        , @UserAccountId);";

                    object[] parameters =
                    {
                        new SqlParameter("@TransactionId", DBExtensions.DBValue(transaction.TransactionId)),
                        new SqlParameter("@PlaidAccountId", DBExtensions.DBValue(transaction.PlaidAccountId)),
                        new SqlParameter("@AccountOwner", DBExtensions.DBValue(transaction.AccountOwner)),
                        new SqlParameter("@Amount", DBExtensions.DBValue(transaction.Amount)),
                        new SqlParameter("@TransactionDate", DBExtensions.DBValue(transaction.TransactionDate)),
                        new SqlParameter("@CurrencyCode", DBExtensions.DBValue(transaction.CurrencyCode)),
                        new SqlParameter("@Address", DBExtensions.DBValue(transaction.Address)),
                        new SqlParameter("@City", DBExtensions.DBValue(transaction.City)),
                        new SqlParameter("@Country", DBExtensions.DBValue(transaction.Country)),
                        new SqlParameter("@PostalCode", DBExtensions.DBValue(transaction.PostalCode)),
                        new SqlParameter("@Region", DBExtensions.DBValue(transaction.Region)),
                        new SqlParameter("@MerchantName", DBExtensions.DBValue(transaction.MerchantName)),
                        new SqlParameter("@Name", DBExtensions.DBValue(transaction.Name)),
                        new SqlParameter("@PaymentMethod", DBExtensions.DBValue(transaction.PaymentMethod)),
                        new SqlParameter("@ReferenceNumber", DBExtensions.DBValue(transaction.ReferenceNumber)),
                        new SqlParameter("@Pending", DBExtensions.DBValue(transaction.Pending)),
                        new SqlParameter("@TransactionCode", DBExtensions.DBValue(transaction.TransactionCode)),
                        new SqlParameter("@UnofficialCurrencyCode", DBExtensions.DBValue(transaction.CurrencyCode)),
                        new SqlParameter("@SpendingFrom", DBExtensions.DBValue(transaction.SpendingFrom)),
                        new SqlParameter("@InternalCategory", DBExtensions.DBValue(transaction.InternalCategory)),
                        new SqlParameter("@AccountId", DBExtensions.DBValue(transaction.PlaidAccountId)),
                        new SqlParameter("@UserAccountId", DBExtensions.DBValue(transaction.UserAccountId)),
                    };

                    await _db.Database.ExecuteSqlRawAsync(sql, parameters);
                }

                //await context.SaveChangesAsync();
                return StatusRM.CreateSuccess(null, "Successfully updated transactions.");
            }
            catch (Exception ex)
            {
                return StatusRM.CreateError(ex);
            }
        }
    }
}