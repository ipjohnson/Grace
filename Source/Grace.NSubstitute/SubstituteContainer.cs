using Grace.DependencyInjection;

namespace Grace.NSubstitute
{
	/// <summary>
	/// This container will create exports for missing types using NSubstitute
	/// It's equvalent to calling .Substitute() on a container
	/// </summary>
	public class SubstituteContainer : DependencyInjectionContainer
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="environment">environment for this container, by default unit test</param>
		/// <param name="comparer">comparer method for container</param>
		/// <param name="disposalScopeProvider">disposal scope provider</param>
		public SubstituteContainer(ExportEnvironment environment = ExportEnvironment.UnitTest,
			ExportStrategyComparer comparer = null,
			IDisposalScopeProvider disposalScopeProvider = null)
			: base(environment, comparer, disposalScopeProvider)
		{
			AddSecondaryLocator(new NSubstituteDependencyLocator());
		}
	}
}