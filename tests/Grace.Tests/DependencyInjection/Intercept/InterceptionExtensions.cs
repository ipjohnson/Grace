using System;
using System.Reflection;
using Castle.DynamicProxy;
using Grace.DependencyInjection;

namespace Grace.Tests.DependencyInjection.Intercept
{
    public static class InterceptExtensions
    {
        public static IFluentDecoratorStrategyConfiguration Intercept<TService, TInterceptor>(
            this IExportRegistrationBlock block) where TInterceptor : IInterceptor
        {
            Type decoratorType;

            var tService = typeof(TService);

            if (tService.GetTypeInfo().IsInterface)
            {
                decoratorType = ProxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(tService, new Type[0],
                    ProxyGenerationOptions.Default);
            }
            else if (tService.GetTypeInfo().IsClass)
            {
                decoratorType = ProxyBuilder.CreateClassProxyTypeWithTarget(tService, new Type[0],
                    ProxyGenerationOptions.Default);
            }
            else
            {
                throw new Exception($"Service type must be interface or class");
            }

            return block.ExportDecorator(decoratorType).As(tService).WithCtorParam<TInterceptor, IInterceptor[]>(i => new IInterceptor[] { i });
        }
        
        private static DefaultProxyBuilder ProxyBuilder => _proxyBuilder ?? (_proxyBuilder = new DefaultProxyBuilder());

        private static DefaultProxyBuilder _proxyBuilder;
    }
}
