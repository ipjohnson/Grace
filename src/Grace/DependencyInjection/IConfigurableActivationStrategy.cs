using System;
using System.Reflection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Configurable Activation Strategy 
    /// </summary>
    public interface IConfigurableActivationStrategy : IActivationStrategy
    {
        /// <summary>
        /// Priority for the strategy
        /// </summary>
        new int Priority { get; set; }

        /// <summary>
        /// lifestyle associated with the strategy
        /// </summary>
        new ICompiledLifestyle Lifestyle { get; set; }

        /// <summary>
        /// Disposal delegate for strategy
        /// </summary>
        object DisposalDelegate { get; set; }

        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <param name="exportType">type to export as</param>
        void AddExportAs(Type exportType);

        /// <summary>
        /// Export as keyed type
        /// </summary>
        /// <param name="exportType">type to export as</param>
        /// <param name="key">export key</param>
        void AddExportAsKeyed(Type exportType, object key);

        /// <summary>
        /// Export as name
        /// </summary>
        /// <param name="name"></param>
        void AddExportAsName(string name);

        /// <summary>
        /// Add condition for strategy
        /// </summary>
        /// <param name="condition">condition</param>
        void AddCondition(ICompiledCondition condition);

        /// <summary>
        /// Add member injection selector
        /// </summary>
        /// <param name="selector">member selector</param>
        void MemberInjectionSelector(IMemberInjectionSelector selector);

        /// <summary>
        /// Add method injection info
        /// </summary>
        /// <param name="methodInjectionInfo"></param>
        void MethodInjectionInfo(MethodInjectionInfo methodInjectionInfo);

        /// <summary>
        /// Delegate to enrich strategy with
        /// </summary>
        /// <param name="enrichmentDelegate">enrichment delegate</param>
        void EnrichmentDelegate(object enrichmentDelegate);

        /// <summary>
        /// Constructor parameter
        /// </summary>
        /// <param name="info"></param>
        void ConstructorParameter(ConstructorParameterInfo info);
        
        /// <summary>
        /// Constructor to use when creating
        /// </summary>
        ConstructorInfo SelectedConstructor { get; set; }

        /// <summary>
        /// Constructor selection method
        /// </summary>
        IConstructorExpressionCreator ConstructorSelectionMethod { get; set; }

        /// <summary>
        /// IS strategy externally owned
        /// </summary>
        new bool ExternallyOwned { get; set; }

        /// <summary>
        /// Add metadata to strategy
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetMetadata(object key, object value);

        /// <summary>
        /// Method that's called when the type is activated
        /// </summary>
        MethodInjectionInfo ActivationMethod { get; set; }

        /// <summary>
        /// If not null then instance will be created in new scope.
        /// </summary>
        string CustomScopeName { get; set; }
    }
}
