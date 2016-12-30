using System;
using System.Web;

namespace Grace.MVC.Extensions
{
    /// <summary>
    /// This Http Module dispose any scope created 
    /// </summary>
    public class DisposableHttpModule : IHttpModule
    {
        /// <summary>Initializes a module and prepares it to handle requests.</summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application </param>
        public void Init(HttpApplication context)
        {
            context.EndRequest += ContextOnEndRequest;
        }

        /// <summary>Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.</summary>
        public void Dispose()
        {
        }

        private void ContextOnEndRequest(object sender, EventArgs eventArgs)
        {
            MVCDisposalScopeProvider.DisposeScopeInHttpContext();
        }
    }
}
