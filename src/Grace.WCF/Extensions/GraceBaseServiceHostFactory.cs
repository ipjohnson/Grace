using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Grace.DependencyInjection;

namespace Grace.WCF.Extensions
{
    /// <summary>
    /// Service factory base
    /// </summary>
	public abstract class GraceBaseServiceHostFactory : ServiceHostFactory
	{
		private readonly IExportLocatorScope exportLocator;

        /// <summary>
        /// Default cosntructor
        /// </summary>
        /// <param name="exportLocator"></param>
		protected GraceBaseServiceHostFactory(IExportLocatorScope exportLocator)
		{
			this.exportLocator = exportLocator;
		}

	    /// <summary>Creates a <see cref="T:System.ServiceModel.ServiceHost" /> for a specified type of service with a specific base address. </summary>
	    /// <returns>A <see cref="T:System.ServiceModel.ServiceHost" /> for the type of service specified with a specific base address.</returns>
	    /// <param name="serviceType">Specifies the type of service to host. </param>
	    /// <param name="baseAddresses">The <see cref="T:System.Array" /> of type <see cref="T:System.Uri" /> that contains the base addresses for the service hosted.</param>
	    protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			ServiceHost serviceHost = CreateServiceHostInstance(serviceType, baseAddresses);

			serviceHost.Opening +=
				(sender, args) => serviceHost.Description.Behaviors.Add(new GraceServiceBehavior(exportLocator));

			return serviceHost;
		}

		/// <summary>
		/// Create service host isntance
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="baseAddresses"></param>
		/// <returns></returns>
		protected abstract ServiceHost CreateServiceHostInstance(Type serviceType, Uri[] baseAddresses);
	}
}