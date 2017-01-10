using Grace.DependencyInjection;

namespace Grace.WCF
{
	/// <summary>
	/// This is a singleton used by the default service host to locate services
	/// </summary>
	public static class DefaultExportLocator
	{
		/// <summary>
		/// This can be the container or a child scope
		/// </summary>
		public static IExportLocatorScope Instance { get; set; }
	}
}