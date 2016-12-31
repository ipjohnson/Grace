using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Grace.DependencyInjection;

namespace Grace.WCF.Extensions
{
    /// <summary>
    /// WCF Behavior that sets up DI
    /// </summary>
	public class GraceServiceBehavior : IServiceBehavior
	{
		private readonly IExportLocatorScope _locator;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="locator"></param>
		public GraceServiceBehavior(IExportLocatorScope locator)
		{
			_locator = locator;
		}

	    /// <summary>Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.</summary>
	    /// <param name="serviceDescription">The service description.</param>
	    /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
	    public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

	    /// <summary>Provides the ability to pass custom data to binding elements to support the contract implementation.</summary>
	    /// <param name="serviceDescription">The service description of the service.</param>
	    /// <param name="serviceHostBase">The host of the service.</param>
	    /// <param name="endpoints">The service endpoints.</param>
	    /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
	    public void AddBindingParameters(ServiceDescription serviceDescription,
			ServiceHostBase serviceHostBase,
			Collection<ServiceEndpoint> endpoints,
			BindingParameterCollection bindingParameters)
		{
		}

	    /// <summary>Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.</summary>
	    /// <param name="serviceDescription">The service description.</param>
	    /// <param name="serviceHostBase">The host that is currently being built.</param>
	    public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		    if (serviceDescription == null) throw new ArgumentNullException(nameof(serviceDescription));
		    if (serviceHostBase == null) throw new ArgumentNullException(nameof(serviceHostBase));

			for (var dispatcherIndex = 0; dispatcherIndex < serviceHostBase.ChannelDispatchers.Count; dispatcherIndex++)
			{
				ChannelDispatcherBase dispatcher = serviceHostBase.ChannelDispatchers[dispatcherIndex];
				ChannelDispatcher channelDispatcher = (ChannelDispatcher)dispatcher;

				for (var endpointIndex = 0; endpointIndex < channelDispatcher.Endpoints.Count; endpointIndex++)
				{
					EndpointDispatcher endpointDispatcher = channelDispatcher.Endpoints[endpointIndex];

					endpointDispatcher.DispatchRuntime.InstanceProvider = new GraceInstanceProvider(_locator,
						serviceDescription.ServiceType);
				}
			}
		}
	}
}