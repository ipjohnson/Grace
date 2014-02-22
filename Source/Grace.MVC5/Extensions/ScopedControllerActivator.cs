using System;
using System.Web.Mvc;
using System.Web.Routing;
using Grace.DependencyInjection;

namespace Grace.MVC.Extensions
{
	public class ScopedControllerActivator : DefaultControllerFactory
	{
		private readonly IPerRequestScopeProvider scopeProvider;

		public ScopedControllerActivator(IPerRequestScopeProvider scopeProvider)
		{
			this.scopeProvider = scopeProvider;
		}

		protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
		{
			if (controllerType == null)
			{
				return base.GetControllerInstance(requestContext, null);
			}

			IInjectionScope scope = scopeProvider.ProvideInjectionScope();

			return scope.Locate(controllerType) as IController;
		}
	}
}