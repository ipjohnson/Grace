using System;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
    public interface IConfigurableActivationStrategy : IActivationStrategy
    {
        new int Priority { get; set; }

        new ICompiledLifestyle Lifestyle { get; set; }

        void AddExportAs(Type exportType);

        void AddExportAsKeyed(Type exportType, object key);

        void AddCondition(ICompiledCondition condition);

        void MemberInjectionSelector(IMemeberInjectionSelector selector);

        void EnrichmentDelegate(object enrichmentDelegate);

        void ConstructorParameter(ConstructorParameterInfo info);
        
        new bool ExternallyOwned { get; set; }

        void SetMetadata(object key, object value);
    }
}
