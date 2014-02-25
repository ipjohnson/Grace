using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Logging;

namespace Grace.Data
{
	/// <summary>
	/// C# extensions for complex object
	/// </summary>
   // ReSharper disable once InconsistentNaming
	public static class IComplexObjectExtensions
	{
		private static readonly string supplemental = typeof(IComplexObjectExtensions).ToString();

		/// <summary>
		/// Execute an action on all objects
		/// </summary>
		/// <typeparam name="T">object type</typeparam>
		/// <param name="complexObject">complex object</param>
		/// <param name="executeAction">action to execute</param>
		/// <param name="catchExceptions">catch all exceptions</param>
		public static void ExecuteActionOnAll<T>(this IComplexObject complexObject,
															  Action<T> executeAction,
															  bool catchExceptions = false) where T : class
		{
			if (complexObject is T)
			{
				if (catchExceptions)
				{
					try
					{
						executeAction(complexObject as T);
					}
					catch (Exception exp)
					{
						Logger.Error("Exception thrown while walking hierarchy", supplemental, exp);
					}
				}
				else
				{
					executeAction(complexObject as T);
				}
			}

			IEnumerable<IComplexObject> children = complexObject.Children;

			if (children != null)
			{
				List<IComplexObject> oldChildren =
					new List<IComplexObject>(children);

				foreach (IComplexObject child in oldChildren)
				{
					if (child.Parent == complexObject)
					{
						child.ExecuteActionOnAll(executeAction, catchExceptions);
					}
				}
			}
		}
	}
}
