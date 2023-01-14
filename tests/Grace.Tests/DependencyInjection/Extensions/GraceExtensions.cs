using Grace.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
#if NET6_0_OR_GREATER
using System.Threading.Tasks;
#endif

namespace Grace.Tests.DependencyInjection.Extensions
{
    /// <summary>
    /// Static class for MVC registration.
    /// </summary>
    public static class GraceRegistration
    {
        /// <summary>
        /// Populate the Grace DI container with Microsoft DI service descriptors.
        /// </summary>
        /// <param name="exportLocator">export locator</param>
        /// <param name="descriptors">descriptors</param>
        public static IServiceProvider Populate(this IInjectionScope exportLocator, IEnumerable<ServiceDescriptor> descriptors)
        {
            exportLocator.Configure(c =>
            {
#if NET6_0_OR_GREATER
                c.Export<ServiceProviderIsServiceImpl>().As<IServiceProviderIsService>();
#endif

                c.ExcludeTypeFromAutoRegistration(nameof(Microsoft) + ".*");
                c.Export<GraceServiceProvider>().As<IServiceProvider>().ExternallyOwned();
                c.Export<GraceLifetimeScopeServiceScopeFactory>().As<IServiceScopeFactory>().Lifestyle.Singleton();
                Register(c, descriptors);

            });

            return exportLocator.Locate<IServiceProvider>();
        }

        private static void Register(IExportRegistrationBlock c, IEnumerable<ServiceDescriptor> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                if (descriptor.ImplementationType != null)
                {
                    c.Export(descriptor.ImplementationType)
                        .As(descriptor.ServiceType)
                        .ConfigureLifetime(descriptor.Lifetime);
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    c.ExportFactory(descriptor.ImplementationFactory)
                        .As(descriptor.ServiceType)
                        .ConfigureLifetime(descriptor.Lifetime);
                }
                else
                {
                    c.ExportInstance(descriptor.ImplementationInstance)
                        .As(descriptor.ServiceType)
                        .ConfigureLifetime(descriptor.Lifetime);
                }
            }
        }

        private static IFluentExportStrategyConfiguration ConfigureLifetime(
            this IFluentExportStrategyConfiguration configuration, ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    return configuration.Lifestyle.SingletonPerScope();

                case ServiceLifetime.Singleton:
                    return configuration.Lifestyle.Singleton();
            }

            return configuration;
        }

        private static IFluentExportInstanceConfiguration<T> ConfigureLifetime<T>(
            this IFluentExportInstanceConfiguration<T> configuration, ServiceLifetime lifecycleKind)
        {
            switch (lifecycleKind)
            {
                case ServiceLifetime.Scoped:
                    return configuration.Lifestyle.SingletonPerScope();

                case ServiceLifetime.Singleton:
                    return configuration.Lifestyle.Singleton();
            }

            return configuration;
        }

        /// <summary>
        /// Service provider for Grace
        /// </summary>
        private class GraceServiceProvider 
            : IServiceProvider
            , IDisposable
#if NET6_0_OR_GREATER
            , IAsyncDisposable
#endif
        {
            private readonly IExportLocatorScope _injectionScope;

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="injectionScope"></param>
            public GraceServiceProvider(IExportLocatorScope injectionScope)
            {
                _injectionScope = injectionScope;
            }

            /// <summary>Gets the service object of the specified type.</summary>
            /// <returns>A service object of type <paramref name="serviceType" />.-or- null if there is no service object of type <paramref name="serviceType" />.</returns>
            /// <param name="serviceType">An object that specifies the type of service object to get. </param>
            /// <filterpriority>2</filterpriority>
            public object GetService(Type serviceType)
            {
                return _injectionScope.LocateOrDefault(serviceType, null);
            }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                _injectionScope.Dispose();
            }

#if NET6_0_OR_GREATER
            /// <summary>Asynchronously performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public ValueTask DisposeAsync()
            {
                return _injectionScope.DisposeAsync();
            }
#endif
        }

        /// <summary>
        /// Service scope factory that uses grace
        /// </summary>
        private class GraceLifetimeScopeServiceScopeFactory : IServiceScopeFactory
        {
            private readonly IExportLocatorScope _injectionScope;

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="injectionScope"></param>
            public GraceLifetimeScopeServiceScopeFactory(IExportLocatorScope injectionScope)
            {
                _injectionScope = injectionScope;
            }

            /// <summary>
            /// Create an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceScope" /> which
            /// contains an <see cref="T:System.IServiceProvider" /> used to resolve dependencies from a
            /// newly created scope.
            /// </summary>
            /// <returns>
            /// An <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceScope" /> controlling the
            /// lifetime of the scope. Once this is disposed, any scoped services that have been resolved
            /// from the <see cref="P:Microsoft.Extensions.DependencyInjection.IServiceScope.ServiceProvider" />
            /// will also be disposed.
            /// </returns>
            public IServiceScope CreateScope()
            {
                return new GraceServiceScope(_injectionScope.BeginLifetimeScope());
            }
        }

        /// <summary>
        /// Grace service scope
        /// </summary>
        private class GraceServiceScope : IServiceScope
#if NET6_0_OR_GREATER
            , IAsyncDisposable
#endif
        {
            private readonly IExportLocatorScope _injectionScope;

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="injectionScope"></param>
            public GraceServiceScope(IExportLocatorScope injectionScope)
            {
                _injectionScope = injectionScope;

                ServiceProvider = injectionScope;
            }

            /// <summary>
            /// Service provider
            /// </summary>
            public IServiceProvider ServiceProvider { get; }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                _injectionScope.Dispose();
            }

#if NET6_0_OR_GREATER
            // This code added to correctly and asynchronously implement the disposable pattern.
            public ValueTask DisposeAsync()
            {
                return _injectionScope.DisposeAsync();
            }
#endif
        }


#if NET6_0_OR_GREATER
        private class ServiceProviderIsServiceImpl : IServiceProviderIsService
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
#endif
    }

}
