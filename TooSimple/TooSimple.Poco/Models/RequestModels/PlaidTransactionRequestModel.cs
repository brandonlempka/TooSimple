using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.RequestModels
{
    public class PlaidTransactionRequestModel
    {
        public string AccessToken { get; set; }
        public string[] AccountIds { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Count { get; set; }
        public int Offset { get; set; }

        public PlaidTransactionRequestModel()
        {
            StartDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            EndDate = DateTime.Now.ToString("yyyy-MM-dd");
            Offset = 0;
            Count = 300;
        }

    }
}
