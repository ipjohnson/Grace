using System;
using Grace.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;

namespace Grace.Tests.DependencyInjection.Extensions
{
    /// <summary>
    /// These tests are from microsoft to make sure it conforms to it's container specifications
    /// </summary>
    public class InjectionSpecificationTests : DependencyInjectionSpecificationTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            var container = new DependencyInjectionContainer();

            return container.Populate(serviceCollection);
        }
    }

    internal class ServiceProviderIsServiceImpl : IServiceProviderIsService
    {
        private readonly IExportLocatorScope _exportLocatorScope;

        public ServiceProviderIsServiceImpl(IExportLocatorScope exportLocatorScope)
        {
            _exportLocatorScope = exportLocatorScope;
        }

        public bool IsService(Type serviceType)
        {
            if (serviceType.IsGenericTypeDefinition)
            {
                return false;
            }

            return _exportLocatorScope.CanLocate(serviceType);
        }
    }
}
