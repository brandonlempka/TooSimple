using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Models.ActionModels;
using TooSimple.Models.DataModels;
using TooSimple.Models.DataModels.Plaid;
using TooSimple.Models.ResponseModels;

namespace TooSimple.DataAccessors
{
    public interface IAccountDataAccessor
    {
        Task<AccountListDM> GetAccountDMAsync(string userId, string accountId = "");
        Task<TransactionListDM> GetTransactionListAsync(string userId, int page, int resultsPerPage);
        Task<StatusRM> SavePlaidAccountData(IEnumerable<AccountDM> dataModel);
        Task<StatusRM> SavePlaidTransactionData(IEnumerable<TransactionDM> dataModel);
        Task<StatusRM> UpdateAccountAsync(DashboardSaveAccountAM actionModel);
        Task<StatusRM> SetRelog(string accountId);
        Task<StatusRM> DeleteAccountAsync(string accountId);
        Task<TransactionDM> GetTransactionDMAsync(string transactionId);
        Task<TransactionListDM> GetSpendingFromTransactions(string userId);
        Task<SaveTransactionRM> SaveTransactionAsync(DashboardEditTransactionAM actionModel);
    }
}
