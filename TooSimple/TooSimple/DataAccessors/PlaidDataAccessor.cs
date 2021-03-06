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
using TooSimple.Models.DataModels.Plaid.TokenCreation;
using TooSimple.Models.DataModels.Plaid.TokenExchange;
using TooSimple.Models.DataModels;

namespace TooSimple.DataAccessors
{
    public class PlaidDataAccessor : IPlaidDataAccessor
    {
        private AppSettings _appSettings;
        private string _language = "en";
        private string[] _countries = new string[] { "US" };
        private string[] _products = new string[] { "auth", "transactions", "liabilities" };
        private string _clientName = "Too Simple";
        private string[] _debit_account_filters = new string[] { "checking", "savings", "hsa" };
        private string[] _credit_account_filters = new string[] { "credit card" };

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

                user = new UserDM
                {
                    client_user_id = userId
                },

                //account_filters = new AccountFiltersDM
                //{
                //    depository = new DepositoryDM
                //    {
                //        account_subtypes = _debit_account_filters
                //    },
                //    credit = new CreditDM
                //    {
                //        account_subtypes = _credit_account_filters
                //    }
                //}
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

        public async Task<CreateLinkTokenRM> PublicTokenUpdateAsync(AccountDM account)
        {
            var dataModel = new TEUpdateDM
            {
                client_id = _appSettings.PlaidClientId,
                secret = _appSettings.PlaidSecret,
                country_codes = _countries,
                language = _language,
                access_token = account.AccessToken,
                client_name = _clientName,
                user = new TEUserDM
                {
                    client_user_id = account.UserAccountId
                }
            };

            var requestJson = JsonConvert.SerializeObject(dataModel);
            var url = _appSettings.PlaidBaseUrl + "link/token/create";

            var response = await ApiExtension.GetApiResponse(url, "POST", requestJson);

            var responseModel = JsonConvert.DeserializeObject<CreateLinkTokenRM>(response);
            return responseModel;
        }


        public async Task<PlaidAccountRequestRM> GetAccountBalancesAsync(string accessToken, string[] accountIds)
        {
            try
            {
                var dataModel = new PlaidAccountRequestDM
                {
                    access_token = accessToken,
                    client_id = _appSettings.PlaidClientId,
                    secret = _appSettings.PlaidSecret,
                    options = new PlaidAccountOptionsDM
                    {
                        account_ids = accountIds
                    }
                };

                var requestJson = JsonConvert.SerializeObject(dataModel);
                var url = _appSettings.PlaidBaseUrl + "accounts/balance/get";

                var response = await ApiExtension.GetApiResponse(url, "POST", requestJson);

                return JsonConvert.DeserializeObject<PlaidAccountRequestRM>(response);
            }
            catch(Exception ex)
            {
                return new PlaidAccountRequestRM
                {
                    request_id = ex.ToString()
                };
            }
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
