using System;
using System.Collections.Generic;

namespace Grace.LanguageExtensions
{
	/// <summary>
	/// Extensions for IEnumerable
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public static class IEnumerableExtensions
	{
		/// <summary>
		/// Apply an action to an IEnumerable
		/// </summary>
		/// <typeparam name="T">t type</typeparam>
		/// <param name="enumerable">enumerable</param>
		/// <param name="action">action to apply</param>
		public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (T t in enumerable)
			{
				action(t);
			}
		}
	}
}