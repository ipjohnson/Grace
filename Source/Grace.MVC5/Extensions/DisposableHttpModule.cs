using System;
using System.Web;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.MVC.Extensions
{
	/// <summary>
	/// This Http Module dispose any scope created 
	/// </summary>
	public class DisposableHttpModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			context.EndRequest += ContextOnEndRequest;
		}

		public void Dispose()
		{
		}

		private void ContextOnEndRequest(object sender, EventArgs eventArgs)
		{
			MVCInjectionScopeProvider.DisposeInjectionScope();

			MVCDisposalScopeProvider.DisposeDisposalScope();
		}
	}
}