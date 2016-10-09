using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Static class that offers methods for filtering members
    /// </summary>
    public static class MembersThat
    {
        /// <summary>
        /// Are named a specific name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MembersThatConfiguration AreNamed(string name)
        {
            return new MembersThatConfiguration().AreNamed(name);
        }

        /// <summary>
        /// Member name starts with prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static MembersThatConfiguration StartWith(string prefix)
        {
            return new MembersThatConfiguration().StartsWith(prefix);
        }

        /// <summary>
        /// Member name ends with
        /// </summary>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public static MembersThatConfiguration EndsWith(string postfix)
        {
            return new MembersThatConfiguration().EndsWith(postfix);
        }

        /// <summary>
        /// Is member a property that matches
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static MembersThatConfiguration AreProperty(Func<PropertyInfo, bool> filter = null)
        {
            return new MembersThatConfiguration().AreProperty(filter);
        }
        /// <summary>
        /// True if the member is a method and matches optional filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static MembersThatConfiguration AreMethod(Func<MethodInfo, bool> filter = null)
        {
            return new MembersThatConfiguration().AreMethod(filter);
        }

        /// <summary>
        /// True if the member has a specific attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="attributeFilter"></param>
        /// <returns></returns>
        public static MembersThatConfiguration HaveAttribute<TAttribute>(Func<TAttribute, bool> attributeFilter = null)
            where TAttribute : Attribute
        {
            return new MembersThatConfiguration().HaveAttribute(attributeFilter);
        }
    }
}
