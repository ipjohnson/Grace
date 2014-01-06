using Grace.DependencyInjection.Lifestyle;

namespace Grace.MVC.DependencyInjection
{
	public class WebSharedPerRequestLifestyleProvider : IPerRequestLifestyleProvider
	{
		public ILifestyle ProvideContainer()
		{
			return new WebSharedPerRequestLifestyle();
		}
	}
}