using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Poco.Models.RequestModels;
using TooSimple.Poco.Models.ResponseModels;
using TooSimple.Poco.Models.DataModels;
using TooSimple.Poco.Models.ResponseModels.Plaid;

namespace TooSimple.DataAccessors.Plaid
{
    public interface IPlaidDataAccessor
    {
        Task<CreateLinkTokenRM> CreateLinkTokenAsync(string userId);
        Task<TokenExchangeRM> PublicTokenExchangeAsync(string publicToken);
        Task<CreateLinkTokenRM> PublicTokenUpdateAsync(AccountDM account);
        Task<PlaidAccountRequestRM> GetAccountBalancesAsync(string accessToken, string[] accountIds);
        Task<PlaidTransactionRequestRM> GetTransactionsAsync(PlaidTransactionRequestModel requestModel);
    }
}
