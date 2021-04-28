using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.RequestModels
{
    public class MoveMoneyRequestModel
    {
        public string UserAccountId { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public bool AutoRefill { get; set; }
        public string Note { get; set; }
        public DateTime TransferDate { get; set; }
        public bool AutomatedTransfer { get; set; }
    }
}
