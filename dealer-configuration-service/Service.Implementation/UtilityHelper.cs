using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Implementation
{
    /// <summary>
    /// Class, it is responsible for all utility helpers.
    /// </summary>
    public static class UtilityHelper
    {

        /// <summary>
        /// Convert object value into specific type
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="castTo"></param>
        /// <returns></returns>
        public static dynamic Cast(dynamic obj, Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        }

        /// <summary>
        /// Get query string value
        /// </summary>
        /// <param name="queryPair"></param>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public static string GetQueryStringValue(IEnumerable<KeyValuePair<string, string>> queryPair, string queryName)
        {
            return queryPair.FirstOrDefault(query => string.Compare(query.Key, queryName, true) == 0).Value;
        }

    }
}
