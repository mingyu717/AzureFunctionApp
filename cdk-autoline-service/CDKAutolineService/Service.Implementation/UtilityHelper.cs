using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Service.Implementation
{
    public class UtilityHelper
    {
        /// <summary>
        /// Convert byte content to string.
        /// </summary>
        /// <param name="byteContent">byte content</param>
        /// <returns></returns>
        public static string ByteToString(byte[] byteContent)
        {
            StringBuilder sBinary = new StringBuilder();

            for (int i = 0; i < byteContent.Length; i++)
            {
                sBinary.Append(byteContent[i].ToString("X2")); // hex format
            }

            return sBinary.ToString();
        }

        /// <summary>
        /// Serialize object into string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string SerializeObject(object obj, JsonConverter converters = null)
        {
            return JsonConvert.SerializeObject(obj, converters);
        }

        /// <summary>
        /// Get deserialize object from provided string value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object DeserializeObject(string value)
        {
            return JsonConvert.DeserializeObject(value);
        }

        /// <summary>
        /// Get enum description value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

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