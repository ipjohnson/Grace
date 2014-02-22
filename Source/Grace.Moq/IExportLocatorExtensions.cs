using System;
using Grace.DependencyInjection;

namespace Grace.Moq
{
	/// <summary>
	/// Contains C# extension methods for IExportLocator
	/// </summary>
   // ReSharper disable once InconsistentNaming
	public static class IExportLocatorExtensions
	{
		/// <summary>
		/// Enables Moq on a DependencyInjectionContainer or IInjectionScope
		/// </summary>
		/// <param name="locator"></param>
		public static void Moq(this IExportLocator locator)
		{
			locator.AddSecondaryLocator(new MoqDependencyLocator());

			locator.Configure(c => c.Export<MockCollection>().As<IMockCollection>().AndSingleton());
		}

		/// <summary>
		/// Verify all Mock objects created by the container pass verify 
		/// </summary>
		/// <param name="locator"></param>
		public static void Assert(this IExportLocator locator)
		{
			IMockCollection mockCollection = locator.Locate<IMockCollection>();

			if (mockCollection != null)
			{
				mockCollection.Assert();
			}
			else
			{
				throw new Exception("Moq has not been enabled for this scope");
			}
		}
	}
}