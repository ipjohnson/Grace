using System;
using System.ServiceModel;
using Grace.DependencyInjection;

namespace Grace.WCF.Extensions
{
    /// <summary>
    /// Service host factory
    /// </summary>
	public class GraceServiceHostFactory : GraceBaseServiceHostFactory
	{
        /// <summary>
        /// Default constructor, uses DefaultExportLocator.Instance
        /// </summary>
		public GraceServiceHostFactory() : base(DefaultExportLocator.Instance)
		{
		}

        /// <summary>
        /// Constructor that takes export locator
        /// </summary>
        /// <param name="exportLocator"></param>
		public GraceServiceHostFactory(IExportLocatorScope exportLocator) : base(exportLocator)
		{
		}

        /// <summary>
        /// Creates new service host
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="baseAddresses"></param>
        /// <returns></returns>
		protected override ServiceHost CreateServiceHostInstance(Type serviceType, Uri[] baseAddresses)
		{
			return new ServiceHost(serviceType, baseAddresses);
		}
	}
}