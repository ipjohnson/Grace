using System;
using System.Linq;
using System.Reflection;
using Grace.Data;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Configuration object for filtering members
    /// </summary>
    public class MembersThatConfiguration
    {
        private readonly GenericFilterGroup<MemberInfo> _filters = new GenericFilterGroup<MemberInfo>();

        /// <summary>
        /// Are named a specific name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MembersThatConfiguration AreNamed(string name)
        {
            var notValue = GetNotAndingValue();

            _filters.Add(m => (m.Name == name) == notValue);

            return this;
        }

        /// <summary>
        /// Member name starts with prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public MembersThatConfiguration StartsWith(string prefix)
        {
            var notValue = GetNotAndingValue();

            _filters.Add(m => (m.Name.StartsWith(prefix)) == notValue);

            return this;
        }

        /// <summary>
        /// Member name ends with
        /// </summary>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public MembersThatConfiguration EndsWith(string postfix)
        {
            var notValue = GetNotAndingValue();

            _filters.Add(m => (m.Name.EndsWith(postfix)) == notValue);

            return this;
        }

        /// <summary>
        /// Match a specific member
        /// </summary>
        /// <param name="matchMethod"></param>
        /// <returns></returns>
        public MembersThatConfiguration Match(Func<MemberInfo, bool> matchMethod)
        {
            var notValue = GetNotAndingValue();

            _filters.Add(m => matchMethod(m) == notValue);

            return this;
        }

        /// <summary>
        /// Is member a property that matches
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public MembersThatConfiguration AreProperty(Func<PropertyInfo, bool> filter = null)
        {
            var notValue = GetNotAndingValue();

            Func<MemberInfo, bool> newFilter = m =>
            {
                var p = m as PropertyInfo;

                if (p != null && filter != null)
                {
                    return filter(p) == notValue;
                }

                return (p != null) == notValue;
            };

            _filters.Add(newFilter);

            return this;
        }


        /// <summary>
        /// Matched is the Memember is a Method
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public MembersThatConfiguration AreMethod(Func<MethodInfo, bool> filter = null)
        {
            var notValue = GetNotAndingValue();

            Func<MemberInfo, bool> newFilter = m =>
            {
                var p = m as MethodInfo;

                if (p != null && filter != null)
                {
                    return filter(p) == notValue;
                }

                return (p != null) == notValue;
            };

            _filters.Add(newFilter);

            return this;
        }

        /// <summary>
        /// Have a specific attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="attributeFilter"></param>
        /// <returns></returns>
        public MembersThatConfiguration HaveAttribute<TAttribute>(Func<TAttribute, bool> attributeFilter = null)
            where TAttribute : Attribute
        {
            var notValue = GetNotAndingValue();
            Func<MemberInfo, bool> newFilter;

            if (attributeFilter != null)
            {
                newFilter = t => t.GetCustomAttributes(true).
                    Where(a => ReflectionService.CheckTypeIsBasedOnAnotherType(a.GetType(), typeof(TAttribute))).
                    Any(
                    x =>
                    {
                        var returnValue = false;
                        var attribute =
                            x as TAttribute;

                        if (attribute != null)
                        {
                            returnValue = attributeFilter(attribute);
                        }

                        return returnValue;
                    })
                    == notValue;
            }
            else
            {
                newFilter = t => t.GetCustomAttributes(typeof(TAttribute), true).
                                   Any(a => ReflectionService.CheckTypeIsBasedOnAnotherType(a.GetType(), typeof(TAttribute))) == notValue;
            }

            _filters.Add(newFilter);

            return this;
        }

        /// <summary>
        /// Or together the filters rather than using And
        /// </summary>
        public MembersThatConfiguration Or
        {
            get
            {
                _filters.UseOr = true;

                return this;
            }
        }

        /// <summary>
        /// And together filters rather than using Or
        /// </summary>
        public MembersThatConfiguration And
        {
            get
            {
                if (_filters.UseOr)
                {
                    throw new Exception("Cannot use And with Or");
                }

                _filters.UseOr = false;

                return this;
            }
        }

        /// <summary>
        /// Reverses the logic for the next type filter
        /// </summary>
        public MembersThatConfiguration Not
        {
            get
            {
                _notLogicValue = false;

                return this;
            }
        }

        private bool _notLogicValue = true;

        private bool GetNotAndingValue()
        {
            var tempValue = _notLogicValue;

            _notLogicValue = true;

            return tempValue;
        }

        /// <summary>
        /// Implicitly convert to func
        /// </summary>
        /// <param name="configuration"></param>
        public static implicit operator Func<MemberInfo, bool>(MembersThatConfiguration configuration)
        {
            return configuration._filters;
        }
    }
}
