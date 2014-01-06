using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.MVC.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	public class MVCDisposalScopeProvider : IDisposalScopeProvider
	{
		private const string DISPOSAL_SCOPE_KEY = "DisposalScopeKey";

		public IDisposalScope ProvideDisposalScope(IInjectionScope injectionScope)
		{
			if (HttpContext.Current != null)
			{
				return GetDisposalScope();
			}

			return injectionScope;
		}

		/// <summary>
		/// Disposes the disposal scope
		/// </summary>
		public static void DisposeDisposalScope()
		{
			IDisposalScope returnScope = HttpContext.Current.Items[DISPOSAL_SCOPE_KEY] as IDisposalScope;

			if (returnScope != null)
			{
				returnScope.Dispose();
			}
		}

		/// <summary>
		/// Gets the current disposal scope for the request
		/// </summary>
		/// <returns></returns>
		public static IDisposalScope GetDisposalScope()
		{
			IDisposalScope returnScope = HttpContext.Current.Items[DISPOSAL_SCOPE_KEY] as IDisposalScope;

			if (returnScope == null)
			{
				returnScope = new DisposalScope();

				HttpContext.Current.Items[DISPOSAL_SCOPE_KEY] = returnScope;
			}

			return returnScope;
		}
	}
}
