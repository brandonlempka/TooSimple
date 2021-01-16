using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.EFModels
{
    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TransactionId { get; set; }
        public string PlaidTransactionId { get; set; }
        public string PlaidAccountId { get; set; }
        public string AccountOwner { get; set; }
        public decimal Amount { get; set; }
        public DateTime AuthorizedDate { get; set; }
        public ICollection<TransactionCategory> Categories { get; set; }
        public DateTime TransactionDate { get; set; }
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
        public string PaymentChannel { get; set; }
        public string ByOrderOf { get; set; }
        public string Payee { get; set; }
        public string Payer { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentProcessor { get; set; }
        public string PpdId { get; set; }
        public string Reason { get; set; }
        public string ReferenceNumber { get; set; }
        public bool Pending { get; set; }
        public string PendingTransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionType { get; set; }
        public string UnofficialCurrencyCode { get; set; }
        public string SpendingFrom { get; set; }
        public string InternalCategory { get; set; }
    }
}
