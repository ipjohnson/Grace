using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.Data
{
    public static class IExraDataContainerExtensions
    {
        /// <summary>
        /// Get a value from extra data or return default. will convert 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetExtraDataOrDefaultValue<T>(this IExtraDataContainer container, object key, T defaultValue = default (T))
        {
            var value = container.GetExtraData(key);

            if (value == null)
            {
                return defaultValue;
            }

            if (value is T)
            {
                return (T)value;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
