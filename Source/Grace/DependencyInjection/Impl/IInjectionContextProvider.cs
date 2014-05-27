using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Provide injection contexts for an injection scope
	/// </summary>
	public interface IInjectionContextProvider
	{
		/// <summary>
		/// Provide an injection context
		/// </summary>
		/// <param name="scope">injection scope</param>
		/// <param name="disposalScope">disposal scope</param>
		/// <param name="disposalScopeProvider">disposal scope provider</param>
		/// <returns>new injection context</returns>
		IInjectionContext CreateInjectionContext(IInjectionScope scope,
															  IDisposalScope disposalScope,
															  IDisposalScopeProvider disposalScopeProvider);
	}
}
