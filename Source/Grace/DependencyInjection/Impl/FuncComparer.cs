using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
	public class FuncComparer<TItem> : IComparer<TItem>
	{
		private readonly Func<TItem, IComparable> valueFunc;

		public FuncComparer(Func<TItem, IComparable> valueFunc)
		{
			this.valueFunc = valueFunc;
		}

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