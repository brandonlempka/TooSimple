using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Models.ResponseModels.Plaid;

namespace TooSimple.DataAccessors
{
    public interface IPlaidDataAccessor
    {
        Task<CreateLinkTokenRM> CreateLinkTokenAsync(string userId);
    }
}
