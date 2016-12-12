using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Grace.AspNetCore.MVC
{
    /// <summary>
    /// Custom controller activator that uses container
    /// </summary>
    public class GraceControllerActivator : IControllerActivator
    {
        /// <summary>Creates a controller.</summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.ControllerContext" /> for the executing action.</param>
        public object Create(ControllerContext context)
        {
            return context.HttpContext.RequestServices.GetService(context.ActionDescriptor.ControllerTypeInfo.AsType());
        }

        /// <summary>Releases a controller.</summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.ControllerContext" /> for the executing action.</param>
        /// <param name="controller">The controller to release.</param>
        public void Release(ControllerContext context, object controller)
        {

        }
    }
}
