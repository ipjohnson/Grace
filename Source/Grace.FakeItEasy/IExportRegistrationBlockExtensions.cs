using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.FakeItEasy
{
	/// <summary>
	/// Static extension methods that allow you to configure a Fake export
	/// </summary>
   // ReSharper disable once InconsistentNaming
	public static class IExportRegistrationBlockExtensions
	{
		/// <summary>
		/// Registers a Fake Type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="registrationBlock"></param>
		/// <returns></returns>
		public static IFakeExportStrategyConfiguration<T> Fake<T>(this IExportRegistrationBlock registrationBlock) where T : class
		{
			FakeExportStrategy<T> strategy = new FakeExportStrategy<T>();
			
			registrationBlock.AddExportStrategy(strategy);

			return new FakeExportStrategyConfiguration<T>(strategy);
		}
	}
}
