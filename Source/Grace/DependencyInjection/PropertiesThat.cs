using Grace.DependencyInjection.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Static class that offers methods for filtering properties
    /// </summary>
    public static class PropertiesThat
    {        
        /// <summary>
        /// Match properties that are named a specific thing
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PropertiesThatConfiguration AreNamed(string name)
        {
            return new PropertiesThatConfiguration().AreNamed(name);
        }

        /// <summary>
        /// Member name starts with prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static PropertiesThatConfiguration StartsWith(string prefix)
        {
            return new PropertiesThatConfiguration().StartsWith(prefix);
        }

        /// <summary>
        /// Member name ends with
        /// </summary>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public static PropertiesThatConfiguration EndsWith(string postfix)
        {
            return new PropertiesThatConfiguration().EndsWith(postfix);
        }

        /// <summary>
        /// Have a specific attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="attributeFilter"></param>
        /// <returns></returns>
        public static PropertiesThatConfiguration HaveAttribute<TAttribute>(Func<TAttribute, bool> attributeFilter = null)
            where TAttribute : Attribute
        {
            return new PropertiesThatConfiguration().HaveAttribute(attributeFilter);
        }
    }
}
