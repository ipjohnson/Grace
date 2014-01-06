using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Static class that provides IComparer(T) classes for sorting
	/// </summary>
	public static class SortBy
	{
		public static IComparer<T> ProprtyAscending<T, TProp>(Func<T, TProp> propertyFunc) where TProp : IComparable
		{
			return new PropertyAscendingComparer<T,TProp>(propertyFunc);
		}

		public static IComparer<T> ProprtyDecending<T, TProp>(Func<T, TProp> propertyFunc) where TProp : IComparable
		{
			return new PropertyDescendingComparer<T, TProp>(propertyFunc);
		}
	}
}
