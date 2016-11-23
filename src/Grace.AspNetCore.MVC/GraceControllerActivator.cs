using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Http;

namespace Grace.AspNetCore.MVC
{
    public class GraceControllerActivator : IControllerActivator
    {
        public object Create(ControllerContext context)
        {
            return context.HttpContext.RequestServices.GetService(context.ActionDescriptor.ControllerTypeInfo.AsType());
        }

        public void Release(ControllerContext context, object controller)
        {

        }
    }
}
