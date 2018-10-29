using System;

namespace Processor.Implementation
{
    public class UtilityHelper
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
    }
}