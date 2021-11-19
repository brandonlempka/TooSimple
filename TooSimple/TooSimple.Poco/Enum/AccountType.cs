using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TooSimple.Poco.Enum
{
    public enum AccountType
    {
        UNKNOWN = 0,
        [Description("checking")]
        Checking = 1,
        [Description("credit card")]
        CreditCard = 2
    }
}