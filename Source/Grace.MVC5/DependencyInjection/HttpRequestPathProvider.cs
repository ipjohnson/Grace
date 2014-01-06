using System.Web;
using Grace.DependencyInjection;

namespace Grace.MVC.DependencyInjection
{
	public class HttpRequestPathProvider : IExportValueProvider
	{
		public object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider)
		{
			if (HttpContext.Current != null)
			{
				return HttpContext.Current.Request.Path;
			}

			return null;
		}
	}
}