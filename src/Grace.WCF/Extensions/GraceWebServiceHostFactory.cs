using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Grace.DependencyInjection;

namespace Grace.WCF.Extensions
{
    /// <summary>
    /// Factory for web
    /// </summary>
	public class GraceWebServiceHostFactory : GraceBaseServiceHostFactory
	{
        /// <summary>
        /// Default web host factory, uses DefaultExportLocator.Instance
        /// </summary>
		public GraceWebServiceHostFactory() : base(DefaultExportLocator.Instance)
		{
		}

        /// <summary>
        /// Constructor that takes export locator
        /// </summary>
        /// <param name="exportLocator"></param>
		public GraceWebServiceHostFactory(IExportLocatorScope exportLocator) : base(exportLocator)
		{
		}

	    /// <summary>
	    /// Create service host isntance
	    /// </summary>
	    /// <param name="serviceType"></param>
	    /// <param name="baseAddresses"></param>
	    /// <returns></returns>
	    protected override ServiceHost CreateServiceHostInstance(Type serviceType, Uri[] baseAddresses)
		{
			return new WebServiceHost(serviceType, baseAddresses);
		}
	}
}