using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Grace.DependencyInjection;

namespace Grace.WCF.Extensions
{
	public class GraceWebServiceHostFactory : GraceBaseServiceHostFactory
	{
		public GraceWebServiceHostFactory() : base(DefaultExportLocator.Instance)
		{
		}

		public GraceWebServiceHostFactory(IExportLocator exportLocator) : base(exportLocator)
		{
		}

		public override ServiceHost CreateServiceHostInstance(Type serviceType, Uri[] baseAddresses)
		{
			return new WebServiceHost(serviceType, baseAddresses);
		}
	}
}