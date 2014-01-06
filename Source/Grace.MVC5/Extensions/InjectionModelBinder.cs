using System;
using System.Web.Mvc;
using Grace.DependencyInjection;

namespace Grace.MVC.Extensions
{
	public class InjectionModelBinder : DefaultModelBinder
	{
		private readonly IPerRequestScopeProvider scopeProvider;

		public InjectionModelBinder(IPerRequestScopeProvider scopeProvider)
		{
			this.scopeProvider = scopeProvider;
		}

		protected override object CreateModel(ControllerContext controllerContext,
			ModelBindingContext bindingContext,
			Type modelType)
		{
			IInjectionScope scope = scopeProvider.ProvideInjectionScope();
			object returnValue = scope.Locate(modelType);

			return returnValue ?? base.CreateModel(controllerContext, bindingContext, modelType);
		}
	}
}