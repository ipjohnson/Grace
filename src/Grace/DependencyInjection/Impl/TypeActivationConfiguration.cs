using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Contains the information required to create an activation expression
    /// </summary>
    public class TypeActivationConfiguration
    {
        /// <summary>
        /// list of cosntructor parameters
        /// </summary>
        protected ImmutableLinkedList<ConstructorParameterInfo> ConstructorParametersList = ImmutableLinkedList<ConstructorParameterInfo>.Empty;

        /// <summary>
        /// list of enrichment delegates
        /// </summary>
        protected ImmutableLinkedList<object> EncrichmentDelegateList = ImmutableLinkedList<object>.Empty;

        /// <summary>
        /// list of member injection selectors
        /// </summary>
        protected ImmutableLinkedList<IMemberInjectionSelector> MemberInjectorList = ImmutableLinkedList<IMemberInjectionSelector>.Empty;

        /// <summary>
        /// list of method injection info
        /// </summary>
        protected ImmutableLinkedList<MethodInjectionInfo> MethodInjectionList = ImmutableLinkedList<MethodInjectionInfo>.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="activationStrategy"></param>
        public TypeActivationConfiguration(Type activationType, IActivationStrategy activationStrategy)
        {
            ActivationType = activationType;
            ActivationStrategy = activationStrategy;
            SupportsDecorators = false;
        }

        /// <summary>
        /// Type being activated
        /// </summary>
        public Type ActivationType { get; }

        /// <summary>
        /// Strategy that owns this configuration
        /// </summary>
        public IActivationStrategy ActivationStrategy { get; }

        /// <summary>
        /// Method to call upon activation
        /// </summary>
        public MethodInjectionInfo ActivationMethod { get; set; }

        /// <summary>
        /// Lifestyle for strategy
        /// </summary>
        public ICompiledLifestyle Lifestyle { get; set; }

        /// <summary>
        /// does this strategy support decorators
        /// </summary>
        public bool SupportsDecorators { get; set; }

        /// <summary>
        /// delegate to call when disposed
        /// </summary>
        public object DisposalDelegate { get; set; }

        /// <summary>
        /// Externally owned strategy
        /// </summary>
        public bool ExternallyOwned { get; set; }

        /// <summary>
        /// Constructor to use when creating instance
        /// </summary>
        public ConstructorInfo SelectedConstructor { get; set; }

        /// <summary>
        /// List of constructor parameters
        /// </summary>
        public IEnumerable<ConstructorParameterInfo> ConstructorParameters => ConstructorParametersList;

        /// <summary>
        /// Add constructor parameter to configuration
        /// </summary>
        /// <param name="info"></param>
        public void ConstructorParameter(ConstructorParameterInfo info)
        {
            ConstructorParametersList = ConstructorParametersList.Add(info);
        }

        /// <summary>
        /// List of enrichment delegates
        /// </summary>
        public IEnumerable<object> EnrichmentDelegates => EncrichmentDelegateList.Reverse();

        /// <summary>
        /// Add enrichment delegate
        /// </summary>
        /// <param name="enrichmentDelegate"></param>
        public void EnrichmentDelegate(object enrichmentDelegate)
        {
            EncrichmentDelegateList = EncrichmentDelegateList.Add(enrichmentDelegate);
        }

        /// <summary>
        /// List of member injection selectors
        /// </summary>
        public IEnumerable<IMemberInjectionSelector> MemberInjectionSelectors => MemberInjectorList;

        /// <summary>
        /// Add member injection selector to configuration
        /// </summary>
        /// <param name="selector"></param>
        public void MemberInjectionSelector(IMemberInjectionSelector selector)
        {
            MemberInjectorList = MemberInjectorList.Add(selector);
        }

        /// <summary>
        /// List of methods to inject
        /// </summary>
        public IEnumerable<MethodInjectionInfo> MethodInjections => MethodInjectionList;

        /// <summary>
        /// Constructor selection method
        /// </summary>
        public IConstructorExpressionCreator ConstructorSelectionMethod { get; set; }

        /// <summary>
        /// If not null then type will be created in new scope
        /// </summary>
        public string CustomScopeName { get; set; }

        /// <summary>
        /// add method injection to list
        /// </summary>
        /// <param name="methodInjection"></param>
        public void MethodInjection(MethodInjectionInfo methodInjection)
        {
            if (MethodInjectionList != ImmutableLinkedList<MethodInjectionInfo>.Empty && 
                MethodInjectionList.Any(m => Equals(m.Method, methodInjection.Method)))
            {
                return;
            }

            MethodInjectionList = MethodInjectionList.Add(methodInjection);
        }

        /// <summary>
        /// Clone configuration
        /// </summary>
        /// <param name="activationType"></param>
        /// <returns></returns>
        public TypeActivationConfiguration CloneToType(Type activationType)
        {
            return new TypeActivationConfiguration(activationType, ActivationStrategy)
            {
                EncrichmentDelegateList = EncrichmentDelegateList,
                MemberInjectorList = MemberInjectorList,
                ConstructorParametersList = ConstructorParametersList,
                ActivationMethod = ActivationMethod,
                DisposalDelegate = DisposalDelegate,
                ExternallyOwned = ExternallyOwned,
                CustomScopeName = CustomScopeName
                // leaving out lifestyle as that needs to be thread safe
            };
        }
    }
}
