using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Grace.DependencyInjection;

namespace Grace.MVC.Extensions
{
	public class DisposalScopeControllerActivator : IControllerActivator
	{
		private readonly IInjectionScope injectionScope;

		public DisposalScopeControllerActivator(IInjectionScope injectionScope)
		{
			this.injectionScope = injectionScope;
		}

		public IController Create(RequestContext requestContext, Type controllerType)
		{
			return injectionScope.Locate(controllerType) as IController;
		}
	}
}
