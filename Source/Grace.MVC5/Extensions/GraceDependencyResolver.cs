using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Grace.DependencyInjection;

namespace Grace.MVC.Extensions
{
	public class GraceDependencyResolver : IDependencyResolver
	{
		private readonly IExportLocator container;

		public GraceDependencyResolver(IExportLocator container)
		{
			this.container = container;
		}

		public object GetService(Type serviceType)
		{
			return container.Locate(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return container.LocateAll(serviceType);
		}
	}
}