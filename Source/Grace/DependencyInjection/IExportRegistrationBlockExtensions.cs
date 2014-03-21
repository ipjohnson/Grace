using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Extension methods for registration block
	/// </summary>
   // ReSharper disable once InconsistentNaming
	public static class IExportRegistrationBlockExtensions
	{
		/// <summary>
		/// Ups the priority of partially closed generics based on the number of closed parameters
		/// </summary>
		/// <param name="registrationBlock">registration block</param>
		public static void PrioritizePartiallyClosedGenerics(this IExportRegistrationBlock registrationBlock)
		{
			registrationBlock.AddInspector(new PartiallyClosedGenericPriorityAugmenter());
		}
	}
}
