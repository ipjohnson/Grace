using System;
using System.Collections;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Represents a list of exports that should be black listed
	/// </summary>
	public class BlackList : IEnumerable<string>
	{
		private readonly object lockObject = new object();
		private volatile List<string> blackList;

		/// <summary>
		/// returns a list of class names that are black listed
		/// </summary>
		/// <returns></returns>
		public IEnumerator<string> GetEnumerator()
		{
			return blackList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// adds a new name to the black list
		/// </summary>
		/// <param name="blackedOut"></param>
		public void Add(string blackedOut)
		{
			lock (lockObject)
			{
				List<string> newBlackList = blackList == null ? new List<string>() : new List<string>(blackList);

				newBlackList.Add(blackedOut);

				blackList = newBlackList;
			}
		}

		/// <summary>
		/// Add a new type to black list
		/// </summary>
		/// <param name="blackOutType"></param>
		public void Add(Type blackOutType)
		{
			Add(blackOutType.FullName);
		}

		/// <summary>
		/// Checks to see if an export strategy is on the black list.
		/// </summary>
		/// <param name="exportStrategy"></param>
		/// <returns></returns>
		public bool IsExportStrategyBlackedOut(IExportStrategy exportStrategy)
		{
			List<string> currentList = blackList;

			if (currentList != null)
			{
				return currentList.Contains(exportStrategy.ActivationName);
			}

			return false;
		}
	}
}