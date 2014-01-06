using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Grace.DependencyInjection;

namespace Grace.WCF.Extensions
{
	public class GraceServiceBehavior : IServiceBehavior
	{
		private readonly IExportLocator locator;

		public GraceServiceBehavior(IExportLocator locator)
		{
			this.locator = locator;
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		public void AddBindingParameters(ServiceDescription serviceDescription,
			ServiceHostBase serviceHostBase,
			Collection<ServiceEndpoint> endpoints,
			BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			if (serviceDescription == null)
			{
				throw new ArgumentNullException("serviceDescription");
			}

			if (serviceHostBase == null)
			{
				throw new ArgumentNullException("serviceHostBase");
			}

			for (Int32 dispatcherIndex = 0; dispatcherIndex < serviceHostBase.ChannelDispatchers.Count; dispatcherIndex++)
			{
				ChannelDispatcherBase dispatcher = serviceHostBase.ChannelDispatchers[dispatcherIndex];
				ChannelDispatcher channelDispatcher = (ChannelDispatcher)dispatcher;

				for (Int32 endpointIndex = 0; endpointIndex < channelDispatcher.Endpoints.Count; endpointIndex++)
				{
					EndpointDispatcher endpointDispatcher = channelDispatcher.Endpoints[endpointIndex];

					endpointDispatcher.DispatchRuntime.InstanceProvider = new GraceInstanceProvider(locator,
						serviceDescription.ServiceType);
				}
			}
		}
	}
}