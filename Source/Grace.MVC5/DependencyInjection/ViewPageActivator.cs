using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Grace.DependencyInjection;

namespace Grace.MVC.DependencyInjection
{
    /// <summary>
    /// Activates a new view
    /// </summary>
    public class ViewPageActivator : IViewPageActivator
    {
        private readonly IExportLocator _container;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="container"></param>
        public ViewPageActivator(IExportLocator container)
        {
            _container = container;
        }

        public object Create(ControllerContext controllerContext, Type type)
        {
            // We use the GUID because when ASP.Net compilier re-compile it uses the same type name but different GUID
            object returnObject = _container.Locate(type);

            if (returnObject == null)
            {
                _container.Configure(c => c.Export(typeof(Type)));

                returnObject = _container.Locate(type);
            }

            return returnObject;
        }
    }
}
