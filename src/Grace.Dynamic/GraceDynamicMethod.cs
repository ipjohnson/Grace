using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.Dynamic.Impl;

namespace Grace.Dynamic
{
    public class GraceDynamicMethod : InjectionScopeConfiguration
    {
        public GraceDynamicMethod()
        {
            ConfigureImplementation();
        }
        
        public static GraceDynamicMethod Configuration(Action<InjectionScopeConfiguration> configure = null)
        {
            var config = new GraceDynamicMethod();

            configure?.Invoke(config);

            return config;
        }


        private void ConfigureImplementation()
        {
            Implementation = DynamicImplementation.Clone();
        }

        protected static ImplementationFactory DynamicImplementation;

        static GraceDynamicMethod()
        {
            DynamicImplementation = DefaultImplementation.Clone();

            DynamicImplementation.ExportSingleton<IDynamicMethodTargetCreator>(f => new DynamicMethodTargetCreator());
            DynamicImplementation.ExportSingleton<INewExpressionGenerator>(f => new NewExpressionGenerator());
            DynamicImplementation.ExportSingleton<IConstantExpressionCollector>(f => new ConstantExpressionCollector());
            DynamicImplementation.ExportSingleton<IConstantExpressionGenerator>(f => new ConstantExpressionGenerator());
            DynamicImplementation.ExportSingleton<IMemeberInitExpressionGenerator>(f => new MemeberInitExpressionGenerator());
            DynamicImplementation.ExportSingleton<IArrayInitExpressionGenerator>(f => new ArrayInitExpressionGenerator());
            DynamicImplementation.ExportSingleton<IParameterExpressionGenerator>(f => new ParameterExpressionGenerator());
            DynamicImplementation.ExportSingleton<ICallExpressionGenerator>(f => new CallExpressionGenerator());

            DynamicImplementation.ExportInstance<ILinqToDynamicMethodConverter>(f => new LinqToDynamicMethodConverter(f));

            DynamicImplementation.ExportInstance<IActivationStrategyCompiler>(
                f => new DynamicMethodStrategyCompiler(f.InjectionScope.ScopeConfiguration,
                                               f.Locate<IActivationExpressionBuilder>(),
                                               f.Locate<IAttributeDiscoveryService>(),
                                               f.Locate<ILifestyleExpressionBuilder>(),
                                               f.Locate<IInjectionContextCreator>(),
                                               f.Locate<IExpressionConstants>(),
                                               f.Locate<ILinqToDynamicMethodConverter>())); 
        }
    }
}
