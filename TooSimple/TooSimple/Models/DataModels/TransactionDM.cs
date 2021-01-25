using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels
{
    public class TransactionDM
    {
        public string TransactionId { get; set; }
        public string ReferenceNumber { get; set; }
        public string AccountId { get; set; }
        public string AccountOwner { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string CurrencyCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string MerchantName { get; set; }
        public string Name { get; set; }
        public string PaymentMethod { get; set; }
        public bool Pending { get; set; }
        public string TransactionCode { get; set; }
        public string SpendingFrom { get; set; }
        public string InternalCategory { get; set; }
        public string UserAccountId { get; set; }
    }
}
