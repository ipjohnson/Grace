using System;
using System.Linq;
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


        public static IInjectionScope InterceptAttribute<TAttribute, TInterceptor>(this IInjectionScope scope)
            where TInterceptor : IInterceptor where TAttribute : Attribute
        {
            scope.Configure(c =>
            {
                foreach (var strategy in scope.StrategyCollectionContainer.GetAllStrategies())
                {
                    if (strategy.ActivationType.GetTypeInfo().GetCustomAttribute<TAttribute>() != null)
                    {
                        Type decoratorType;
                        var tService = strategy.ExportAs.First();

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

                        c.ExportDecorator(decoratorType).As(tService).WithCtorParam<TInterceptor, IInterceptor[]>(i => new IInterceptor[] { i })
                           .When.MeetsCondition((s, context) =>
                            {
                                return s.ActivationType.GetTypeInfo().GetCustomAttribute<TAttribute>() != null;
                            });
                    }
                }
            });

            return scope;
        }

        private static DefaultProxyBuilder ProxyBuilder => _proxyBuilder ?? (_proxyBuilder = new DefaultProxyBuilder());

        private static DefaultProxyBuilder _proxyBuilder;
    }

}
