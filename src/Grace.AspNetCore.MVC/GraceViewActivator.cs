using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Grace.AspNetCore.MVC
{
    /// <summary>
    /// Custom view activator that creates view from Grace container
    /// </summary>
    public class GraceViewActivator : IViewComponentActivator
    {
        /// <summary>Instantiates a ViewComponent.</summary>
        /// <param name="context">
        /// The <see cref="T:Microsoft.AspNetCore.Mvc.ViewComponents.ViewComponentContext" /> for the executing <see cref="T:Microsoft.AspNetCore.Mvc.ViewComponent" />.
        /// </param>
        public object Create(ViewComponentContext context)
        {
            return context.ViewContext.HttpContext.RequestServices.GetService(context.ViewComponentDescriptor.TypeInfo.AsType());
        }

        /// <summary>Releases a ViewComponent instance.</summary>
        /// <param name="context">
        /// The <see cref="T:Microsoft.AspNetCore.Mvc.ViewComponents.ViewComponentContext" /> associated with the <paramref name="viewComponent" />.
        /// </param>
        /// <param name="viewComponent">The <see cref="T:Microsoft.AspNetCore.Mvc.ViewComponent" /> to release.</param>
        public void Release(ViewComponentContext context, object viewComponent)
        {
            // do nothing as it will be disposed in the scope
        }
    }
}
