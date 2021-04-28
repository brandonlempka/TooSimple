using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Poco.Models.DataModels;

namespace TooSimple.Poco.Models.ViewModels
{
    public class TransactionListVM
    {
        public string TransactionId { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountOwner { get; set; }
        public decimal? Amount { get; set; }
        public string AmountDisplayValue { get; set; }
        public string AmountDisplayColor { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string TransactionDateDisplayValue { get; set; }
        public string CurrencyCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string MerchantName { get; set; }
        public string Name { get; set; }
        public string PaymentMethod { get; set; }
        public bool? Pending { get; set; }
        public string TransactionCode { get; set; }
        public string SpendingFrom { get; set; }
        public string InternalCategory { get; set; }

        public TransactionListVM(TransactionDM x, string accountName = "")
        {
            AccountId = x.AccountId;
            AccountOwner = x.AccountOwner;
            AccountName = accountName;
            Address = x.Address;
            Amount = x.Amount * -1;
            AmountDisplayValue = x.Amount.HasValue ? (x.Amount.Value * -1).ToString("c") : "$0.00";
            City = x.City;
            Country = x.Country;
            CurrencyCode = x.CurrencyCode;
            InternalCategory = x.InternalCategory;
            MerchantName = x.MerchantName;
            Name = x.Name;
            PaymentMethod = x.PaymentMethod;
            Pending = x.Pending;
            PostalCode = x.PostalCode;
            Region = x.Region;
            SpendingFrom = x.SpendingFrom ?? "Ready to Spend";
            TransactionCode = x.TransactionCode;
            TransactionDate = x.TransactionDate;
            TransactionDateDisplayValue = x.TransactionDate?.ToString("MM/dd/yyyy");
            TransactionId = x.TransactionId;
        }
    }
}
