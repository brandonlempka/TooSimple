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
        Task<StatusRM> SavePlaidAccountData(IEnumerable<AccountDM> dataModel);
        Task<StatusRM> SavePlaidTransactionData(IEnumerable<TransactionDM> dataModel);
        Task<StatusRM> UpdateAccountAsync(DashboardEditAccountAM actionModel);
        Task<StatusRM> DeleteAccountAsync(string accountId);
    }
}
