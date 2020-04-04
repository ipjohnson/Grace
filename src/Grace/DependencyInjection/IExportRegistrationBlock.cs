using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents a block of exports that will be registered
    /// </summary>
    public interface IExportRegistrationBlock
    {
        /// <summary>
        /// Scope this registration block is for
        /// </summary>
        IInjectionScope OwningScope { get; }

        /// <summary>
        /// Add your own custom activation strategy
        /// </summary>
        /// <param name="activationStrategy">activation strategy</param>
        void AddActivationStrategy(IActivationStrategy activationStrategy);

        /// <summary>
        /// Add your own strategy provider, usually used by 3rd party libraries to provide their own custom export types
        /// </summary>
        /// <param name="strategyProvider">strategy provider</param>
        void AddExportStrategyProvider(IExportStrategyProvider strategyProvider);

        /// <summary>
        /// Add injection inspector that will be called to inspect all exports, wrappers and decorators (apply cross cutting configuration with an inspector)
        /// </summary>
        /// <param name="inspector">inspector</param>
        void AddInspector(IActivationStrategyInspector inspector);

        /// <summary>
        /// Add missing export strategy provider
        /// </summary>
        /// <param name="provider"></param>
        void AddMissingExportStrategyProvider(IMissingExportStrategyProvider provider);

        /// <summary>
        /// Add missing dependency expression provider
        /// </summary>
        /// <param name="provider"></param>
        void AddMissingDependencyExpressionProvider(IMissingDependencyExpressionProvider provider);

        /// <summary>
        /// Add IInjectionValueProvider allowing the developer to override the normal behavior for creating an injection value
        /// </summary>
        /// <param name="provider"></param>
        void AddInjectionValueProvider(IInjectionValueProvider provider);

        /// <summary>
        /// Add IMemberInjectionSelctor that selects 
        /// </summary>
        /// <param name="selector"></param>
        void AddMemberInjectionSelector(IMemberInjectionSelector selector);

        /// <summary>
        /// Add configuration module
        /// </summary>
        /// <param name="module"></param>
        void AddModule(IConfigurationModule module);

        /// <summary>
        /// Export a specific type
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <returns>export configuration</returns>
        IFluentExportStrategyConfiguration<T> Export<T>();

        /// <summary>
        /// Export a specific type (open generics allowed)
        /// </summary>
        /// <param name="type">type to export</param>
        /// <returns>export configuration</returns>
        IFluentExportStrategyConfiguration Export(Type type);

        /// <summary>
        /// Export a set of types
        /// </summary>
        /// <param name="types">types to export</param>
        /// <returns></returns>
        IExportTypeSetConfiguration Export(IEnumerable<Type> types);

        /// <summary>
        /// Export a type that will be used as a decorator for exports
        /// </summary>
        /// <param name="type">decorator type</param>
        /// <returns></returns>
        IFluentDecoratorStrategyConfiguration ExportDecorator(Type type);

        /// <summary>
        /// Export a piece of logic that will be used to decorate exports upon creation
        /// </summary>
        /// <typeparam name="T">type to decorate</typeparam>
        /// <param name="apply">decorator logic</param>
        /// <param name="applyAfterLifestyle"></param>
        IFluentDecoratorFactoryStrategyConfiguration ExportDecorator<T>(Func<T, T> apply, bool applyAfterLifestyle = true);
        
        /// <summary>
        /// Export an expression tree
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<TResult> ExportExpression<TResult>(Expression<Func<TResult>> expression);

        /// <summary>
        /// Export a specific type
        /// </summary>
        /// <typeparam name="TResult">exported type</typeparam>
        /// <param name="factory">export factory</param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<TResult> ExportFactory<TResult>(Func<TResult> factory);

        /// <summary>
        /// Export a specific type that requires some dependency
        /// </summary>
        /// <typeparam name="TIn">dependency type</typeparam>
        /// <typeparam name="TResult">export type</typeparam>
        /// <param name="factory">export function</param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<TResult> ExportFactory<TIn, TResult>(Func<TIn, TResult> factory);

        /// <summary>
        /// Export a specific type that requires some dependencies
        /// </summary>
        /// <typeparam name="T1">dependency one</typeparam>
        /// <typeparam name="T2">dependency two</typeparam>
        /// <typeparam name="TResult">export type</typeparam>
        /// <param name="factory">export factory</param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, TResult>(Func<T1, T2, TResult> factory);

        /// <summary>
        /// Export a specific type that requires some dependencies
        /// </summary>
        /// <typeparam name="T1">dependency one</typeparam>
        /// <typeparam name="T2">dependency two</typeparam>
        /// <typeparam name="T3">dependency three</typeparam>
        /// <typeparam name="TResult">export type</typeparam>
        /// <param name="factory">export factory</param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> factory);

        /// <summary>
        /// Export a specific type that requires some dependencies
        /// </summary>
        /// <typeparam name="T1">dependency one</typeparam>
        /// <typeparam name="T2">dependency two</typeparam>
        /// <typeparam name="T3">dependency three</typeparam>
        /// <typeparam name="T4">dependency four</typeparam>
        /// <typeparam name="TResult">export type</typeparam>
        /// <param name="factory">export factory</param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> factory);

        /// <summary>
        /// Export a specific type that requires some dependencies
        /// </summary>
        /// <typeparam name="T1">dependency one</typeparam>
        /// <typeparam name="T2">dependency two</typeparam>
        /// <typeparam name="T3">dependency three</typeparam>
        /// <typeparam name="T4">dependency four</typeparam>
        /// <typeparam name="T5">dependency five</typeparam>
        /// <typeparam name="TResult">export type</typeparam>
        /// <param name="factory">export factory</param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> factory);
        
        /// <summary>
        /// Export a specific value
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <param name="instance">instance to export</param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<T> ExportInstance<T>(T instance);

        /// <summary>
        /// Export a specific type using a function
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <param name="instanceFunc">function to create instance</param>
        /// <returns></returns>
        [Obsolete("Please use ExportFactor<T>")]
        IFluentExportInstanceConfiguration<T> ExportInstance<T>(Func<T> instanceFunc);

        /// <summary>
        /// Export a specific type using an IExportLocatorScope
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <param name="instanceFunc">instance func</param>
        /// <returns></returns>
        [Obsolete("Please use ExportFactor<IExportLocatorScope, T>")]
        IFluentExportInstanceConfiguration<T> ExportInstance<T>(Func<IExportLocatorScope, T> instanceFunc);

        /// <summary>
        /// Export a specific type using IExportLocatorScope and StaticInjectionContext
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <param name="instanceFunc">isntance func</param>
        /// <returns></returns>
        [Obsolete("Please use ExportFactor<IExportLocatorScope, StaticInjectionContext, T>")]
        IFluentExportInstanceConfiguration<T> ExportInstance<T>(Func<IExportLocatorScope, StaticInjectionContext, T> instanceFunc);

        /// <summary>
        /// Export a specific type using IExportLocatorScope, StaticInjectionContext and IInjectionContext
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <param name="instanceFunc">isntance func</param>
        /// <returns></returns>
        [Obsolete("Please use ExportFactor<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T>")]
        IFluentExportInstanceConfiguration<T> ExportInstance<T>(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T> instanceFunc);

        /// <summary>
        /// Export a type to be used as a wrapper rather than export (types like Func(), Owned, Meta are wrapper types)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IFluentWrapperStrategyConfiguration ExportWrapper(Type type);

        /// <summary>
        /// Test if a type is exported
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="excludeStrategy"></param>
        /// <returns></returns>
        bool IsExported(Type type, object key = null, ICompiledExportStrategy excludeStrategy = null);

        /// <summary>
        /// Clears exports from registration block
        /// </summary>
        /// <param name="exportFilter"></param>
        /// <returns></returns>
        bool ClearExports(Func<ICompiledExportStrategy, bool> exportFilter = null);
        
    }
}
