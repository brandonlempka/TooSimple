using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels
{
    public class FundingHistoryListDM
    {
        public IEnumerable<FundingHistoryDM> FundingHistories { get; set; }
    }
}
