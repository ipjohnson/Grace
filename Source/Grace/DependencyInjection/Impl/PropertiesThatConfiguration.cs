using Grace.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    public class PropertiesThatConfiguration
    {
        private readonly List<Func<PropertyInfo, bool>> _filters = new List<Func<PropertyInfo, bool>>();
        private bool _useOr = false;
        
        /// <summary>
        /// Match properties that are named a specific thing
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PropertiesThatConfiguration AreNamed(string name)
        {
            bool notValue = GetNotAndingValue();

            _filters.Add(p => (p.Name == name) == notValue);

            return this;
        }

        /// <summary>
        /// Member name starts with prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public PropertiesThatConfiguration StartsWith(string prefix)
        {
            bool notValue = GetNotAndingValue();

            _filters.Add(m => (m.Name.StartsWith(prefix)) == notValue);

            return this;
        }

        /// <summary>
        /// Member name ends with
        /// </summary>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public PropertiesThatConfiguration EndsWith(string postfix)
        {
            bool notValue = GetNotAndingValue();

            _filters.Add(m => (m.Name.EndsWith(postfix)) == notValue);

            return this;
        }

        /// <summary>
        /// Have a specific attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="attributeFilter"></param>
        /// <returns></returns>
        public PropertiesThatConfiguration HaveAttribute<TAttribute>(Func<TAttribute, bool> attributeFilter = null)
            where TAttribute : Attribute
        {
            bool notValue = GetNotAndingValue();
            Func<PropertyInfo, bool> newFilter;

            if (attributeFilter != null)
            {
                newFilter = t => t.GetCustomAttributes(true).
                    Where(a => ReflectionService.CheckTypeIsBasedOnAnotherType(a.GetType(), typeof(TAttribute))).
                    Any(
                    x =>
                    {
                        bool returnValue = false;
                        TAttribute attribute =
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
                                   Any(a => ReflectionService.CheckTypeIsBasedOnAnotherType(a.GetType(), typeof(TAttribute)))
                                   == notValue;
            }

            _filters.Add(newFilter);

            return this;
        }

        /// <summary>
        /// Or together the filters rather than using And
        /// </summary>
        public PropertiesThatConfiguration Or
        {
            get
            {
                _useOr = true;

                return this;
            }
        }

        /// <summary>
        /// And together filters rather than using Or
        /// </summary>
        public PropertiesThatConfiguration And
        {
            get
            {
                if (_useOr)
                {
                    throw new Exception("Cannot use And with Or");
                }

                _useOr = false;

                return this;
            }
        }

        /// <summary>
        /// Reverses the logic for the next type filter
        /// </summary>
        public PropertiesThatConfiguration Not
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
            bool tempValue = _notLogicValue;

            _notLogicValue = true;

            return tempValue;
        }

        /// <summary>
        /// Implicitly convert to func
        /// </summary>
        /// <param name="configuration"></param>
        public static implicit operator Func<PropertyInfo, bool>(PropertiesThatConfiguration configuration)
        {
            return new GenericFilterGroup<PropertyInfo>(configuration._filters.ToArray()) { UseOr = configuration._useOr };
        }
    }
}
