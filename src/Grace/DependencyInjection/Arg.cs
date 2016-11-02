using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Arg helper
    /// </summary>
    public class Arg
    {
        /// <summary>
        /// Any arguement of type T
        /// </summary>
        /// <typeparam name="T">type of arg</typeparam>
        /// <returns>default T value</returns>
        public static T Any<T>()
        {
            return default(T);
        }
    }
}
