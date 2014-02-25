using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// Combines two life styles to try the first then the second 
	/// </summary>
	/// <typeparam name="TMain"></typeparam>
	/// <typeparam name="TSecondary"></typeparam>
	public sealed class HybridLifestyle<TMain,TSecondary> : ILifestyle where TMain : ILifestyle, new()
																					where TSecondary : ILifestyle, new()
	{
		private bool disposed;
		private readonly ILifestyle mainLifestyle;
		private readonly ILifestyle secondaryLifestyle;

		/// <summary>
		/// Default constructor
		/// </summary>
		public HybridLifestyle()
		{
			mainLifestyle = new TMain();
			secondaryLifestyle = new TSecondary();
		}

		/// <summary>
		/// Constructor takes two lifestyles
		/// </summary>
		/// <param name="main">main lifestyle</param>
		/// <param name="secondary">secondary lifestyle</param>
		public HybridLifestyle(TMain main, TSecondary secondary)
		{
			mainLifestyle = main;
			secondaryLifestyle = secondary;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (disposed)
			{
				return;
			}

			disposed = true;

			mainLifestyle.Dispose();
			secondaryLifestyle.Dispose();
		}

		/// <summary>
		/// If true then the container will allow the dependencies to be located in down facing scopes
		/// otherwise they will only be resolved in the current scope and in upward scopes (i.e. parent scope)
		/// </summary>
		public bool Transient
		{
			get { return mainLifestyle.Transient & secondaryLifestyle.Transient; }
		}

		/// <summary>
		/// This method is called by the export strategy when attempting to locate an export
		/// </summary>
		/// <param name="creationDelegate"></param>
		/// <param name="injectionScope"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportStrategy"></param>
		/// <returns></returns>
		public object Locate(ExportActivationDelegate creationDelegate,
			IInjectionScope injectionScope,
			IInjectionContext injectionContext,
			IExportStrategy exportStrategy)
		{
			object returnValue = mainLifestyle.Locate(creationDelegate, injectionScope, injectionContext, exportStrategy) ??
			                     secondaryLifestyle.Locate(creationDelegate, injectionScope, injectionContext, exportStrategy);

			return returnValue;
		}

		/// <summary>
		/// This method is used to clone a Lifestyle container
		/// </summary>
		/// <returns></returns>
		public ILifestyle Clone()
		{
			return new HybridLifestyle<TMain, TSecondary>((TMain)mainLifestyle.Clone(),(TSecondary)secondaryLifestyle.Clone());
		}
	}
}
