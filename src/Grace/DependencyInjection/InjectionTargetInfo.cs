using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// information for a specific instance of a type being injected
    /// </summary>
    public class InjectionTargetInfo
    {
        private readonly IAttributeDiscoveryService _attributeDiscoveryService;

        /// <summary>
        /// Default cosntructor
        /// </summary>
        /// <param name="attributeDiscoveryService"></param>
        /// <param name="injectionType"></param>
        /// <param name="requestingStrategy"></param>
        /// <param name="injectionTarget"></param>
        /// <param name="injectionTargetName"></param>
        /// <param name="injectionDependencyType"></param>
        /// <param name="locatedType"></param>
        /// <param name="isRequired"></param>
        /// <param name="defaultValue"></param>
        /// <param name="uniqueId"></param>
        public InjectionTargetInfo(IAttributeDiscoveryService attributeDiscoveryService,
                                   Type injectionType,
                                   IActivationStrategy requestingStrategy,
                                   object injectionTarget,
                                   string injectionTargetName,
                                   RequestType injectionDependencyType,
                                   Type locatedType,
                                   bool isRequired,
                                   object defaultValue,
                                   string uniqueId)
        {
            _attributeDiscoveryService = attributeDiscoveryService;
            InjectionType = injectionType;
            RequestingStrategy = requestingStrategy;
            InjectionTarget = injectionTarget;
            InjectionDependencyType = injectionDependencyType;
            LocateType = locatedType;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
            UniqueId = uniqueId;
            InjectionTargetName = injectionTargetName;
        }

        /// <summary>
        /// This is the type that is being injected into 
        /// </summary>
        public Type InjectionType { get; }

        /// <summary>
        /// Requesting strategy
        /// </summary>
        public IActivationStrategy RequestingStrategy { get; set; }

        /// <summary>
        /// These are the attributes for the class that it's being injected into
        /// </summary>
        public IEnumerable<Attribute> InjectionTypeAttributes => _attributeDiscoveryService.GetAttributes(InjectionType);
        
        /// <summary>
        /// The PropertyInfo or ParameterInfo that is being injected
        /// </summary>
        public object InjectionTarget { get; }

        /// <summary>
        /// Name of target being injected, parameter name, property name, or fieldName
        /// </summary>
        public string InjectionTargetName { get; }

        /// <summary>
        /// Type of injection being done, constructor, property, or method
        /// </summary>
        public RequestType InjectionDependencyType { get; }

        /// <summary>
        /// Attributes associated with the target PropertyInfo or ParameterInfo that is provided
        /// </summary>
        public IEnumerable<Attribute> InjectionTargetAttributes => _attributeDiscoveryService.GetAttributes(InjectionTarget);

        /// <summary>
        /// Attributes on the Constructor, Method, or Property being injected
        /// </summary>
        public IEnumerable<Attribute> InjectionMemberAttributes
        {
            get
            {
                var parameterInfo = InjectionTarget as ParameterInfo;
                
                return _attributeDiscoveryService.GetAttributes(parameterInfo != null ? parameterInfo.Member : InjectionTarget);
            }
        }

        /// <summary>
        /// Locate type being used
        /// </summary>
        public Type LocateType { get; }

        /// <summary>
        /// Is the injection required
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// Default value for injection
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// Unique Id for request
        /// </summary>
        public string UniqueId { get; }
    }
}
