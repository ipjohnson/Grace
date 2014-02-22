using Grace.DependencyInjection.Lifestyle;

namespace Grace.WCF.DependencyInjection
{
	public class WCFSharedPerRequestLifestyleProvider : IPerRequestLifestyleProvider
	{
		public ILifestyle ProvideContainer()
		{
			return new WCFSharedPerRequestLifestyle();
		}
	}
}