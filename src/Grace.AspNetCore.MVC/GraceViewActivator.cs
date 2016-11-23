using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Grace.AspNetCore.MVC
{
    public class GraceViewActivator : IViewComponentActivator
    {
        public object Create(ViewComponentContext context)
        {
            return context.ViewContext.HttpContext.RequestServices.GetService(context.ViewComponentDescriptor.TypeInfo.AsType());
        }

        public void Release(ViewComponentContext context, object viewComponent)
        {
            // do nothing as it will be disposed in the scope
        }
    }
}
