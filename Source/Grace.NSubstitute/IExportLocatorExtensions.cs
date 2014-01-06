using Grace.DependencyInjection;

namespace Grace.NSubstitute
{
	/// <summary>
	/// C# extension class to enable NSubstitute on a Grace container or scope
	/// </summary>
   // ReSharper disable once InconsistentNaming
	public static class IExportLocatorExtensions
	{
		/// <summary>
		/// Enables NSubstitute on a DependencyInjectionContainer or IInjectionScope
		/// </summary>
		/// <param name="locator"></param>
		public static void Substitute(this IExportLocator locator)
		{
			locator.AddSecondaryLocator(new NSubstituteDependencyLocator());
		}
	}
}