using Grace.DependencyInjection;

namespace Grace.Moq
{
	/// <summary>
	/// A container that will supply missing exports with Moq objects.
	/// The same functionality can be had by calling container.Moq()
	/// </summary>
	public class MoqContainer : DependencyInjectionContainer
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="environment">environment for this container, by default unit test</param>
		/// <param name="comparer">comparer method for container</param>
		/// <param name="disposalScopeProvider">disposal scope provider</param>
		public MoqContainer(ExportEnvironment environment = ExportEnvironment.UnitTest,
			ExportStrategyComparer comparer = null,
			IDisposalScopeProvider disposalScopeProvider = null)
			: base(environment, comparer, disposalScopeProvider)
		{
			AddSecondaryLocator(new MoqDependencyLocator());

			Configure(c => c.Export<MockCollection>().As<IMockCollection>().AndSingleton());
		}
	}
}