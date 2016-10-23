﻿using System;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Generic filter group 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericFilterGroup<T>
    {
        private ImmutableLinkedList<Func<T, bool>> _typeFilters;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="typeFilters"></param>
        public GenericFilterGroup(params Func<T, bool>[] typeFilters)
        {
            if (typeFilters == null) throw new ArgumentNullException(nameof(typeFilters));

            _typeFilters = typeFilters.Length == 0 ? 
                           ImmutableLinkedList<Func<T, bool>>.Empty : 
                           ImmutableLinkedList.From(typeFilters);
        }

        /// <summary>
        /// Or together the filters rather than And them
        /// </summary>
        public bool UseOr { get; set; }

        public void Add(Func<T, bool> filter)
        {
            _typeFilters = _typeFilters.Add(filter);
        }

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
                foreach (Func<T, bool> typeFilter in _typeFilters)
                {
                    if (typeFilter(type))
                    {
                        return true;
                    }
                }

                return _typeFilters == ImmutableLinkedList<Func<T, bool>>.Empty;
            }

            foreach (Func<T, bool> typeFilter in _typeFilters)
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