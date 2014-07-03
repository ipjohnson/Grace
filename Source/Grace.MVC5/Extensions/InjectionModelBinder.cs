using System;
using System.Web.Mvc;
using Grace.DependencyInjection;

namespace Grace.MVC.Extensions
{
    /// <summary>
    /// Model binder that injects dependencies
    /// </summary>
	public class InjectionModelBinder : DefaultModelBinder
	{
		private readonly IPerRequestScopeProvider scopeProvider;

        /// <summary>
        /// Default constructir
        /// </summary>
        /// <param name="scopeProvider">per request scope provider</param>
		public InjectionModelBinder(IPerRequestScopeProvider scopeProvider)
		{
			this.scopeProvider = scopeProvider;
		}

        /// <summary>
        /// Creates the specified model type by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        /// A data object of the specified type.
        /// </returns>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param><param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param><param name="modelType">The type of the model object to return.</param>
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