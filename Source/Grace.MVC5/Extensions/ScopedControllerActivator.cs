using System;
using System.Web.Mvc;
using System.Web.Routing;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;

namespace Grace.MVC.Extensions
{
	public class ScopedControllerActivator : IControllerActivator
	{
		private readonly IPerRequestScopeProvider scopeProvider;

		public ScopedControllerActivator(IPerRequestScopeProvider scopeProvider)
		{
			this.scopeProvider = scopeProvider;
		}

		public IController Create(RequestContext requestContext, Type controllerType)
		{
			IInjectionScope scope = scopeProvider.ProvideInjectionScope();

			return scope.Locate(controllerType) as IController;
		}
	}
}