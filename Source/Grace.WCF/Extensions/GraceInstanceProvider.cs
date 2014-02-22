using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Grace.DependencyInjection;

namespace Grace.WCF.Extensions
{
	public class GraceInstanceProvider : IInstanceProvider
	{
		private readonly IExportLocator exportLocator;
		private readonly Type serviceType;

		public GraceInstanceProvider(IExportLocator exportLocator, Type serviceType)
		{
			this.exportLocator = exportLocator;
			this.serviceType = serviceType;
		}

		public object GetInstance(InstanceContext instanceContext)
		{
			return GetInstance(instanceContext, null);
		}

		public object GetInstance(InstanceContext instanceContext, Message message)
		{
			object returnValue = exportLocator.Locate(serviceType);

			if (returnValue == null)
			{
				throw new Exception("Could not create new instance of " + serviceType.FullName);
			}

			return returnValue;
		}

		public void ReleaseInstance(InstanceContext instanceContext, object instance)
		{
		}
	}
}