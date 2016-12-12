using System;
using System.Collections.Generic;

namespace Grace.Utilities
{
    /// <summary>
    /// Helper class that implement IComparer 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> _comparer;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="comparer"></param>
        public GenericComparer(Comparison<T> comparer)
        {
            _comparer = comparer;
        }

        /// <summary>
        /// Constructor that takes method for getting comparable properties from T
        /// </summary>
        /// <param name="comparer"></param>
        public GenericComparer(Func<T,IComparable> comparer)
        {
            _comparer = (x,y) => Comparer<IComparable>.Default.Compare(comparer(x),comparer(y));
        }

        /// <summary>Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.</returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(T x, T y)
        {
            return _comparer(x, y);
        }
    }
}
