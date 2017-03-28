using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.Dynamic.Impl;

namespace Grace.Dynamic
{
    /// <summary>
    /// Configuration class for using IL generation in liue of Linq Expressions
    /// </summary>
    public class GraceDynamicMethod : InjectionScopeConfiguration
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public GraceDynamicMethod() : base(DynamicImplementation.Clone())
        {

        }
        
        /// <summary>
        /// Creates a configuration object for using IL Generation instead of Linq Expressions
        /// </summary>
        /// <param name="configure">configuration delegate, can be null</param>
        /// <returns>configuration object</returns>
        public static GraceDynamicMethod Configuration(Action<InjectionScopeConfiguration> configure = null)
        {
            var config = new GraceDynamicMethod();

            configure?.Invoke(config);

            return config;
        }
        
        /// <summary>
        /// Implementation factory for dynamic IL generation
        /// </summary>
        protected static ImplementationFactory DynamicImplementation;

        /// <summary>
        /// Static constructor to setup dynamic implementation
        /// </summary>
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
            DynamicImplementation.ExportSingleton<IAssignExpressionGenerator>(f => new AssignExpressionGenerator());

            DynamicImplementation.ExportInstance<ILinqToDynamicMethodConverter>(f => new LinqToDynamicMethodConverter(f));

            DynamicImplementation.ExportInstance<IActivationStrategyCompiler>(
                f => new DynamicMethodStrategyCompiler(f.InjectionScope.ScopeConfiguration,
                                               f.Locate<IActivationExpressionBuilder>(),
                                               f.Locate<IAttributeDiscoveryService>(),
                                               f.Locate<IDefaultStrategyExpressionBuilder>(),
                                               f.Locate<IInjectionContextCreator>(),
                                               f.Locate<IExpressionConstants>(),
                                               f.Locate<IInjectionStrategyDelegateCreator>(),
                                               f.Locate<ILinqToDynamicMethodConverter>())); 
        }
    }
}
