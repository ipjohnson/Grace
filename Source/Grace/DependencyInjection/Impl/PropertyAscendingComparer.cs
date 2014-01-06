using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
	public class PropertyAscendingComparer<T, TProp> : IComparer<T> where TProp : IComparable
	{
		private Func<T, TProp> valueFunc;

		public PropertyAscendingComparer(Func<T, TProp> valueFunc)
		{
			this.valueFunc = valueFunc;
		}

		public int Compare(T x, T y)
		{
			IComparable xComparable = null;
			IComparable yComparable = null;

			if (x != null)
			{
				xComparable = valueFunc(x);
			}

			if (y != null)
			{
				yComparable = valueFunc(y);
			}

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
