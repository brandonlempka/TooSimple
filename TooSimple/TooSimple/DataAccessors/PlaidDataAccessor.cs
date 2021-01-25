using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Models.DataModels.Plaid;
using TooSimple.Models.ResponseModels.Plaid;
using TooSimple.Extensions;
using Newtonsoft.Json;
using TooSimple.Models.ResponseModels;
using TooSimple.Models.RequestModels;

namespace TooSimple.DataAccessors
{
    public class PlaidDataAccessor : IPlaidDataAccessor
    {
        private AppSettings _appSettings;
        private string _language = "en";
        private string[] _countries = new string[] { "US" };
        private string[] _products = new string[] { "auth", "transactions" };
        private string _clientName = "Too Simple";

        public PlaidDataAccessor(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<CreateLinkTokenRM> CreateLinkTokenAsync(string userId)
        {
            var dataModel = new CreateLinkTokenDM
            {
                client_id = _appSettings.PlaidClientId,
                secret = _appSettings.PlaidSecret,
                client_name = _clientName,
                country_codes = _countries,
                language = _language,
                products = _products,

                user = new CreateLinkTokenUserDM
                {
                    client_user_id = userId
                }
            };

            var requestJson = JsonConvert.SerializeObject(dataModel);
            var url = _appSettings.PlaidBaseUrl + "link/token/create";

            var response = await ApiExtension.GetApiResponse(url, "POST", requestJson);

            return JsonConvert.DeserializeObject<CreateLinkTokenRM>(response);
        }

        public async Task<TokenExchangeRM> PublicTokenExchangeAsync(string publicToken)
        {
            var dataModel = new TokenExchangeDM
            {
                client_id = _appSettings.PlaidClientId,
                secret = _appSettings.PlaidSecret,
                public_token = publicToken
            };

            var requestJson = JsonConvert.SerializeObject(dataModel);
            var url = _appSettings.PlaidBaseUrl + "item/public_token/exchange";

            var response = await ApiExtension.GetApiResponse(url, "POST", requestJson);

            var responseModel = JsonConvert.DeserializeObject<TokenExchangeRM>(response);
            return responseModel;
        }

        public async Task<PlaidAccountRequestRM> GetAccountBalancesAsync(string accessToken, string[] accountIds)
        {
            var dataModel = new PlaidAccountRequestDM
            {
                access_token = accessToken,
                client_id = _appSettings.PlaidClientId,
                secret = _appSettings.PlaidSecret,
                options = new PlaidOptionsDM
                {
                    account_ids = accountIds
                }
            };

            var requestJson = JsonConvert.SerializeObject(dataModel);
            var url = _appSettings.PlaidBaseUrl + "accounts/balance/get";

            var response = await ApiExtension.GetApiResponse(url, "POST", requestJson);

            return JsonConvert.DeserializeObject<PlaidAccountRequestRM>(response);
        }

        public async Task<PlaidTransactionRequestRM> GetTransactionsAsync(PlaidTransactionRequestModel requestModel)
        {
            var dataModel = new PlaidTransactionRequestDM
            {
                access_token = requestModel.AccessToken,
                client_id = _appSettings.PlaidClientId,
                secret = _appSettings.PlaidSecret,
                start_date = requestModel.StartDate,
                end_date = requestModel.EndDate,
                options = new PlaidTransactionRequestOptionsDM
                {
                    account_ids = requestModel.AccountIds,
                    count = requestModel.Count,
                    offset = requestModel.Offset
                }
            };

            var requestJson = JsonConvert.SerializeObject(dataModel);
            var url = _appSettings.PlaidBaseUrl + "transactions/get";

            var response = await ApiExtension.GetApiResponse(url, "POST", requestJson);

            return JsonConvert.DeserializeObject<PlaidTransactionRequestRM>(response);
        }
    }
}
