using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Grace.DependencyInjection;

namespace Grace.WCF.Extensions
{
	public abstract class GraceBaseServiceHostFactory : ServiceHostFactory
	{
		private IExportLocator exportLocator;

		protected GraceBaseServiceHostFactory(IExportLocator exportLocator)
		{
			this.exportLocator = exportLocator;
		}

		protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			ServiceHost serviceHost = CreateServiceHostInstance(serviceType, baseAddresses);

			serviceHost.Opening +=
				(sender, args) => serviceHost.Description.Behaviors.Add(new GraceServiceBehavior(exportLocator));

			return serviceHost;
		}

		public abstract ServiceHost CreateServiceHostInstance(Type serviceType, Uri[] baseAddresses);
	}
}