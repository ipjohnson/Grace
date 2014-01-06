using System;
using System.ServiceModel;
using Grace.DependencyInjection;

namespace Grace.WCF.Extensions
{
	public class GraceServiceHostFactory : GraceBaseServiceHostFactory
	{
		public GraceServiceHostFactory() : base(DefaultExportLocator.Instance)
		{
		}

		public GraceServiceHostFactory(IExportLocator exportLocator) : base(exportLocator)
		{
		}

		public override ServiceHost CreateServiceHostInstance(Type serviceType, Uri[] baseAddresses)
		{
			return new ServiceHost(serviceType, baseAddresses);
		}
	}
}