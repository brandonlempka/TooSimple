using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.RequestModels
{
    public class PlaidTransactionRequestModel
    {
        public string AccessToken { get; set; }
        public string[] AccountIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Count { get; set; }
        public int Offset { get; set; }

        public PlaidTransactionRequestModel()
        {
            StartDate = DateTime.Now.AddDays(-30);
            EndDate = DateTime.Now;
        }

    }
}
