using Grace.Data.Immutable;
using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Contains the information required to create an activation expression
    /// </summary>
    public class TypeActivationConfiguration
    {
        protected ImmutableLinkedList<ConstructorParameterInfo> ConstructorParametersList = ImmutableLinkedList<ConstructorParameterInfo>.Empty;
        protected ImmutableLinkedList<object> EncrichmentDelegateList = ImmutableLinkedList<object>.Empty;
        protected ImmutableLinkedList<IMemeberInjectionSelector> MemberInjectorList = ImmutableLinkedList<IMemeberInjectionSelector>.Empty;

        public TypeActivationConfiguration(Type activationType)
        {
            ActivationType = activationType;
            SupportsDecorators = false;
        }

        public Type ActivationType { get; }

        public ICompiledLifestyle Lifestyle { get; set; }

        public bool SupportsDecorators { get; set; }

        public object DisposalDelegate { get; set; }

        public IEnumerable<ConstructorParameterInfo> ConstructorParameters => ConstructorParametersList;

        public void ConstructorParameter(ConstructorParameterInfo info)
        {
            ConstructorParametersList = ConstructorParametersList.Add(info);
        }

        public IEnumerable<object> EnrichmentDelegates => EncrichmentDelegateList.Reverse();

        public void EnrichmentDelegate(object enrichmentDelegate)
        {
            EncrichmentDelegateList = EncrichmentDelegateList.Add(enrichmentDelegate);
        }

        public IEnumerable<IMemeberInjectionSelector> MemberInjectionSelectors => MemberInjectorList;

        public void MemberInjectionSelector(IMemeberInjectionSelector selector)
        {
            MemberInjectorList = MemberInjectorList.Add(selector);
        }

        public TypeActivationConfiguration CloneToType(Type activationType)
        {
            return new TypeActivationConfiguration(activationType)
            {
                EncrichmentDelegateList = EncrichmentDelegateList,
                MemberInjectorList = MemberInjectorList,
                // leaving out lifestyle as that needs to be thread safe
            };
        }
    }
}
