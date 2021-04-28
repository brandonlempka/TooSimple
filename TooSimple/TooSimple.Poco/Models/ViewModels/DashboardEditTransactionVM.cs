using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ViewModels
{
    public class DashboardEditTransactionVM
    {
        public string TransactionId { get; set; }
        public string AccountId { get; set; }
        public string AccountOwner { get; set; }
        public decimal Amount { get; set; }
        public DateTime? AuthorizedDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string CurrencyCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string StoreNumber { get; set; }
        public string MerchantName { get; set; }
        public string Name { get; set; }
        public bool? Pending { get; set; }
        public string TransactionType { get; set; }
        public string SpendingFromId { get; set; }
        public List<SelectListItem> Goals { get; set; }
        public string InternalCategory { get; set; }
        public string UserAccountId { get; set; }
    }
}
