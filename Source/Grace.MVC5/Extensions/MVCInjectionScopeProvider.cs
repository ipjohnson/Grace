using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Grace.DependencyInjection;

namespace Grace.MVC.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	public class MVCInjectionScopeProvider : IPerRequestScopeProvider
	{
		private const string INJECTION_SCOPE_KEY = "WebRequestInjectionScope";
		private readonly IInjectionScope injectionScope;

		/// <summary>
		/// DEfault constructor
		/// </summary>
		/// <param name="injectionScope">injection scope for this provider</param>
		public MVCInjectionScopeProvider(IInjectionScope injectionScope)
		{
			this.injectionScope = injectionScope;
		}

		/// <summary>
		/// Provides an injection scope for the current http context
		/// </summary>
		/// <returns></returns>
		public IInjectionScope ProvideInjectionScope()
		{
			IInjectionScope returnScope = null;

			if (HttpContext.Current != null)
			{
				returnScope = HttpContext.Current.Items[INJECTION_SCOPE_KEY] as IInjectionScope;

				if (returnScope == null)
				{
					returnScope = injectionScope.CreateChildScope();

					HttpContext.Current.Items[INJECTION_SCOPE_KEY] = returnScope;
				}
			}
			else
			{
				returnScope = injectionScope;
			}
			return returnScope;
		}

		/// <summary>
		/// Dispose the current injection scope for this web request
		/// </summary>
		public static void DisposeInjectionScope()
		{
			IInjectionScope returnScope = HttpContext.Current.Items[INJECTION_SCOPE_KEY] as IInjectionScope;

			if (returnScope != null)
			{
				returnScope.Dispose();

				HttpContext.Current.Items[INJECTION_SCOPE_KEY] = null;
			}
		}
	}
}
