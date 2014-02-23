using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Comparer class that can be used to compare a particular property on a type of object
	/// </summary>
	/// <typeparam name="TItem">item type</typeparam>
	public class FuncComparer<TItem> : IComparer<TItem>
	{
		private readonly Func<TItem, IComparable> valueFunc;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="valueFunc">property access function</param>
		public FuncComparer(Func<TItem, IComparable> valueFunc)
		{
			this.valueFunc = valueFunc;
		}

		/// <summary>
		/// Compare function
		/// </summary>
		/// <param name="x">x item</param>
		/// <param name="y">y item</param>
		/// <returns>int value representing which one if greater</returns>
		public int Compare(TItem x, TItem y)
		{
			IComparable xComparable = valueFunc(x);
			IComparable yComparable = valueFunc(y);

			if (xComparable != null)
			{
				return xComparable.CompareTo(yComparable);
			}

			if (yComparable == null)
			{
				return 0;
			}

			return -1;
		}
	}
}