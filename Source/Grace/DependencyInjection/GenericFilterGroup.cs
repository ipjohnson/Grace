using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Generic filter group 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericFilterGroup<T>
    {
        private readonly Func<T, bool>[] typeFilters;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="typeFilters"></param>
        public GenericFilterGroup(params Func<T, bool>[] typeFilters)
        {
            if (typeFilters == null)
            {
                throw new ArgumentNullException("typeFilters");
            }

            this.typeFilters = typeFilters;
        }

        /// <summary>
        /// Or together the filters rather than And them
        /// </summary>
        public bool UseOr { get; set; }

        /// <summary>
        /// Automatically convert from TypefilterGroup to Func(Type,bool)
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static implicit operator Func<T, bool>(GenericFilterGroup<T> group)
        {
            return group.InternalFilter;
        }

        protected bool InternalFilter(T type)
        {
            if (UseOr)
            {
                foreach (Func<T, bool> typeFilter in typeFilters)
                {
                    if (typeFilter(type))
                    {
                        return true;
                    }
                }

                return false;
            }

            foreach (Func<T, bool> typeFilter in typeFilters)
            {
                if (!typeFilter(type))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
