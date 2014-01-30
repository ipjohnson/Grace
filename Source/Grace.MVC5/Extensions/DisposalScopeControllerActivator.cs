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
	public class DisposalScopeControllerActivator : DefaultControllerFactory
	{
		private readonly IInjectionScope injectionScope;

		public DisposalScopeControllerActivator(IInjectionScope injectionScope)
		{
			this.injectionScope = injectionScope;
		}

        	protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        	{
            		if (controllerType == null)
            		{
                		return base.GetControllerInstance(requestContext, null);
            		}

            		return injectionScope.Locate(controllerType) as IController;
        	}
	}
}
