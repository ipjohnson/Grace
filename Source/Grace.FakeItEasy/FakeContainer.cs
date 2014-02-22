using Grace.DependencyInjection;

namespace Grace.FakeItEasy
{
	/// <summary>
	/// Fake Container creates missing Exports using Fake It Easy
	/// </summary>
	public class FakeContainer : DependencyInjectionContainer
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="environment">environment for the container to run in, by default UnitTest</param>
		/// <param name="comparer">Comparer to be used to sort strategies</param>
		/// <param name="disposalScopeProvider">disposal scope provider</param>
		public FakeContainer(ExportEnvironment environment = ExportEnvironment.UnitTest,
			ExportStrategyComparer comparer = null,
			IDisposalScopeProvider disposalScopeProvider = null)
			: base(environment, comparer, disposalScopeProvider)
		{
			AddSecondaryLocator(new FakeExportLocator());

			Configure(c => c.Export<FakeCollection>().As<IFakeCollection>().AndSingleton());
		}
	}
}