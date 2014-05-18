using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Disposable injection contexts can be used to resolve disposable object in scope
	/// </summary>
	public class DisposableInjectionContext : InjectionContext, IDisposable
	{
		private bool disposed;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="requestingScope">requesting scope</param>
		public DisposableInjectionContext(IInjectionScope requestingScope) : base(requestingScope)
		{
			DisposalScope = new DisposalScope();
		}

		/// <summary>
		/// Dispose of the context
		/// </summary>
		public void Dispose()
		{
			if (disposed)
			{
				return;
			}

			disposed = true;

			if (DisposalScope != null)
			{
				DisposalScope.Dispose();
			}
		}
	}
}
