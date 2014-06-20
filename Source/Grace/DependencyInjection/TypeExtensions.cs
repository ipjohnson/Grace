using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Extension class for Type
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Syntax glue to allow you to call type.Match(TypesThat.HaveAttribute(typeof(Attribute)))
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="typeFilter">type filter (TypesThat will work here)</param>
        /// <returns>bool value</returns>
        public static bool Matches(this Type type, Func<Type, bool> typeFilter)
        {
            if (typeFilter == null)
            {
                throw new ArgumentNullException("typeFilter");
            }

            return typeFilter(type);
        }
    }
}
