using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
	public class PropertyDescendingComparer<T, TProp> : IComparer<T> where TProp : IComparable
	{
		private Func<T, TProp> valueFunc;

		public PropertyDescendingComparer(Func<T, TProp> valueFunc)
		{
			this.valueFunc = valueFunc;
		}

		public int Compare(T x, T y)
		{
			IComparable comparableX = null;
			IComparable comparableY = null;

			if (x != null)
			{
				comparableX = valueFunc(x);
			}

			if (y != null)
			{
				comparableY = valueFunc(y);
			}

			if (comparableX != null)
			{
				int compareValue = comparableX.CompareTo(comparableY);

				if (compareValue > 0)
				{
					return -1;
				}
				if (compareValue < 0)
				{
					return 1;
				}

				return 0;
			}

			if (comparableY == null)
			{
				return 0;
			}

			return 1;
		}
	}
}
