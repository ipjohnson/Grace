using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Static class that provides IComparer(T) classes for sorting
	/// </summary>
	public static class SortBy
	{
		/// <summary>
		/// Creates IComparer&lt;T&gt; for a specific property, ascending in order
		/// </summary>
		/// <typeparam name="T">Item type</typeparam>
		/// <typeparam name="TProp">property type</typeparam>
		/// <param name="propertyFunc">delegate that fetch TProp off of T</param>
		/// <returns>comparer</returns>
		public static IComparer<T> ProprtyAscending<T, TProp>(Func<T, TProp> propertyFunc) where TProp : IComparable
		{
			return new PropertyAscendingComparer<T, TProp>(propertyFunc);
		}

		/// <summary>
		/// Creates IComparer&lt;T&gt; for a specific property, decending in order
		/// </summary>
		/// <typeparam name="T">Item type</typeparam>
		/// <typeparam name="TProp">property type</typeparam>
		/// <param name="propertyFunc">delegate that fetch TProp off of T</param>
		/// <returns>comparer</returns>
		public static IComparer<T> ProprtyDecending<T, TProp>(Func<T, TProp> propertyFunc) where TProp : IComparable
		{
			return new PropertyDescendingComparer<T, TProp>(propertyFunc);
		}
	}
}