using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Extensions
{
    public class DBExtensions
    {
        public static object DBValue(object value)
        {
            if (value == null)
                return DBNull.Value;
            return value;
        }
    }
}
