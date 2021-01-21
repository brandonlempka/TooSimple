using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ResponseModels.Plaid
{
    public class PlaidPaymentMetaRM
    {
        public object by_order_of { get; set; }
        public object payee { get; set; }
        public object payer { get; set; }
        public object payment_method { get; set; }
        public object payment_processor { get; set; }
        public object ppd_id { get; set; }
        public object reason { get; set; }
        public object reference_number { get; set; }
    }
}
