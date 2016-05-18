using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This class creates all injection kernels and contains all the configuration for all kernels
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class InjectionKernelManager
	{
		private readonly ExportStrategyComparer comparer;
		private readonly object kernelsLock = new object();
		private Dictionary<string, InjectionKernel> kernels = new Dictionary<string, InjectionKernel>();
		private IInjectionScope rootScope;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="container">container for the kernel manager</param>
		/// <param name="comparer">used to compare to export strategies for which one should be used</param>
		/// <param name="blackList">export strategy black list</param>
		public InjectionKernelManager(DependencyInjectionContainer container,
			ExportStrategyComparer comparer)
		{
			Container = container;

			this.comparer = comparer;
		}
        
		/// <summary>
		/// Container this kernel manager is associated with
		/// </summary>
		public DependencyInjectionContainer Container { get; private set; }

		/// <summary>
		/// This method must be called before you configure any named kernels
		/// </summary>
		/// <param name="newRootScope"></param>
		public void SetRootScope(IInjectionScope newRootScope)
		{
			rootScope = newRootScope;
		}

		/// <summary>
		/// Allows you to configure a base kernel of a particular name
		/// </summary>
		/// <param name="kernelName">name of the kernel being configured</param>
		/// <param name="registrationDelegate">registration delegate to call configure with</param>
		public void Configure(string kernelName, ExportRegistrationDelegate registrationDelegate)
		{
			InjectionKernel kernel;

			if (!kernels.TryGetValue(kernelName, out kernel))
			{
				if (rootScope == null)
				{
					throw new Exception("SetRootScope must be called before configuring any named kernels");
				}

                kernel = new InjectionKernel(this, rootScope, kernelName, new KernelConfiguration());

				lock (kernelsLock)
				{
					Dictionary<string, InjectionKernel> newKernels = new Dictionary<string, InjectionKernel>(kernels);

					newKernels[kernelName] = kernel;

					kernels = newKernels;
				}
			}

			kernel.Configure(registrationDelegate);
		}

		/// <summary>
		/// Create a new Kernel of a particular name
		/// </summary>
		/// <param name="parentKernel">the parent kernel</param>
		/// <param name="kernelName">name of the kernel to create</param>
		/// <param name="registrationDelegate"></param>
		/// <param name="parentScopeProvider"></param>
		/// <param name="scopeProvider"></param>
		/// <returns></returns>
		public InjectionKernel CreateNewKernel(InjectionKernel parentKernel,
			string kernelName,
			ExportRegistrationDelegate registrationDelegate, 
            IDisposalScopeProvider parentScopeProvider,
            IDisposalScopeProvider scopeProvider,
            IKernelConfiguration configuration)
		{
			InjectionKernel newKernel;

			if (string.IsNullOrEmpty(kernelName))
			{
				newKernel = new InjectionKernel(this, parentKernel, kernelName, configuration);
			}
			else
			{
				InjectionKernel foundKernel;

				if (kernels.TryGetValue(kernelName, out foundKernel))
				{
                   newKernel = foundKernel.Clone(parentKernel, parentScopeProvider, scopeProvider);
                }
				else
				{
                    newKernel = new InjectionKernel(this, parentKernel, kernelName, configuration);
				}
			}

			if (registrationDelegate != null)
			{
				newKernel.Configure(registrationDelegate);
			}

			return newKernel;
		}
	}
}