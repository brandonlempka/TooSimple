namespace TooSimple.Poco.Models.DataModels.Plaid
{
    public class PlaidTransactionRequestOptionsDM
    {
        public string[] account_ids { get; set; }
        public int count { get; set; }
        public int offset { get; set; }
    }
}