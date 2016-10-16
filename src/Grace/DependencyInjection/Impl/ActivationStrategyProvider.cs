using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Impl.FactoryStrategies;
using Grace.DependencyInjection.Impl.InstanceStrategies;

namespace Grace.DependencyInjection.Impl
{
    public interface IActivationStrategyCreator
    {

        ICompiledDecoratorStrategy GetCompiledDecoratorStrategy(Type activationType);

        ICompiledExportStrategy GetCompiledExportStrategy(Type exportType);

        ICompiledExportStrategy GetConstantStrategyFromConfiguration<T>(T value);
        
        ICompiledExportStrategy GetFactoryStrategy<TResult>(Func<TResult> factory);

        ICompiledExportStrategy GetFactoryStrategy<T1, TResult>(Func<T1, TResult> factory);

        ICompiledExportStrategy GetFactoryStrategy<T1, T2, TResult>(Func<T1, T2, TResult> factory);

        ICompiledExportStrategy GetFactoryStrategy<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> factory);

        ICompiledExportStrategy GetFactoryStrategy<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> factory);

        ICompiledExportStrategy GetFactoryStrategy<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> factory);

        ICompiledDecoratorStrategy GetFuncDecoratorStrategy<T>(Func<T, T> func);

        ICompiledExportStrategy GetFuncStrategy<T>(Func<T> func);

        ICompiledExportStrategy GetFuncWithScopeStrategy<T>(Func<IExportLocatorScope, T> func);

        ICompiledExportStrategy GetFuncWithStaticContextStrategy<T>(Func<IExportLocatorScope, StaticInjectionContext, T> func);

        ICompiledExportStrategy GetFuncWithInjectionContextStrategy<T>(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T> func);

        IConfigurableCompiledWrapperStrategy GetCompiledWrapperStrategy(Type type);

        IExportTypeSetConfiguration GetTypeSetConfiguration(IEnumerable<Type> types);
    }

    public class ActivationStrategyProvider : IActivationStrategyCreator
    {
        private readonly IInjectionScope _injectionScope;
        private readonly ILifestyleExpressionBuilder _exportExpressionBuilder;

        public ActivationStrategyProvider(IInjectionScope injectionScope,
                                          ILifestyleExpressionBuilder exportExpressionBuilder)
        {
            _injectionScope = injectionScope;
            _exportExpressionBuilder = exportExpressionBuilder;
        }

        public virtual IConfigurableCompiledWrapperStrategy GetCompiledWrapperStrategy(Type type)
        {
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                return new GenericCompiledWrapperStrategy(type, _injectionScope, _exportExpressionBuilder);
            }

            return new CompiledWrapperStrategy(type, _injectionScope, _exportExpressionBuilder);
        }

        public virtual IExportTypeSetConfiguration GetTypeSetConfiguration(IEnumerable<Type> types)
        {
            return new ExportTypeSetConfiguration(this, types);
        }

        public virtual ICompiledExportStrategy GetCompiledExportStrategy(Type exportType)
        {
            if (exportType.GetTypeInfo().IsGenericTypeDefinition)
            {
                return new GenericCompiledExportStrategy(exportType, _injectionScope, _exportExpressionBuilder);
            }

            return new CompiledExportStrategy(exportType, _injectionScope, _exportExpressionBuilder);
        }

        public virtual ICompiledDecoratorStrategy GetCompiledDecoratorStrategy(Type activationType)
        {
            return new CompiledDecoratorStrategy(activationType, _injectionScope, _exportExpressionBuilder);
        }

        public virtual ICompiledExportStrategy GetConstantStrategyFromConfiguration<T>(T value)
        {
            return new ConstantInstanceExportStrategy<T>(value, _injectionScope);
        }

        public virtual ICompiledExportStrategy GetFactoryStrategy<TResult>(Func<TResult> factory)
        {
            return new FactoryNoArgStrategy<TResult>(factory, _injectionScope);
        }

        public virtual ICompiledExportStrategy GetFuncStrategy<T>(Func<T> func)
        {
            return new FuncInstanceExportStrategy<T>(func, _injectionScope);
        }

        public virtual ICompiledExportStrategy GetFuncWithScopeStrategy<T>(Func<IExportLocatorScope, T> func)
        {
            return new FuncWithScopeInstanceExportStrategy<T>(func, _injectionScope);
        }

        public virtual ICompiledExportStrategy GetFuncWithStaticContextStrategy<T>(Func<IExportLocatorScope, StaticInjectionContext, T> func)
        {
            return new FuncWithStaticContextInstanceExportStrategy<T>(func, _injectionScope);
        }

        public virtual ICompiledExportStrategy GetFuncWithInjectionContextStrategy<T>(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T> func)
        {
            return new FuncWithInjectionContextInstanceExportStrategy<T>(func, _injectionScope);
        }

        public virtual ICompiledExportStrategy GetFactoryStrategy<T1, TResult>(Func<T1, TResult> factory)
        {
            return new FactoryOneArgStrategy<T1, TResult>(factory, _injectionScope);
        }

        public virtual ICompiledExportStrategy GetFactoryStrategy<T1, T2, TResult>(Func<T1, T2, TResult> factory)
        {
            return new FactoryTwoArgStrategy<T1, T2, TResult>(factory, _injectionScope);
        }

        public virtual ICompiledExportStrategy GetFactoryStrategy<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> factory)
        {
            return new FactoryThreeArgStrategy<T1, T2, T3, TResult>(factory, _injectionScope);
        }

        public virtual ICompiledExportStrategy GetFactoryStrategy<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> factory)
        {
            return new FactoryFourArgStrategy<T1, T2, T3, T4, TResult>(factory, _injectionScope);
        }

        public virtual ICompiledExportStrategy GetFactoryStrategy<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> factory)
        {
            return new FactoryFiveArgStrategy<T1, T2, T3, T4, T5, TResult>(factory, _injectionScope);
        }

        public virtual ICompiledDecoratorStrategy GetFuncDecoratorStrategy<T>(Func<T, T> func)
        {
            return new CompiledInitializationDecoratorStrategy<T>(func, _injectionScope);
        }
    }
}
