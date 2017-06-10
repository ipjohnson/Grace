using System;
using System.Diagnostics;
using Grace.Data;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Dependency type
    /// </summary>
    public enum DependencyType
    {
        /// <summary>
        /// for constructor parameter
        /// </summary>
        ConstructorParameter,

        /// <summary>
        /// Property
        /// </summary>
        Property,

        /// <summary>
        /// Field
        /// </summary>
        Field,

        /// <summary>
        /// Method parameter
        /// </summary>
        MethodParameter,
    }

    /// <summary>
    /// Represents a dependency needed for an activation strategy
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplayString) + ",nq}")]
    public class ActivationStrategyDependency
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="activationStrategy"></param>
        /// <param name="injectionInfo"></param>
        /// <param name="typeBeingImported"></param>
        /// <param name="memberName"></param>
        /// <param name="hasValueProvider"></param>
        /// <param name="hasFilter"></param>
        /// <param name="isSatisfied"></param>
        public ActivationStrategyDependency(DependencyType dependencyType, IActivationStrategy activationStrategy, object injectionInfo, Type typeBeingImported, string memberName, bool hasValueProvider, bool hasFilter, bool isSatisfied)
        {
            DependencyType = dependencyType;
            ActivationStrategy = activationStrategy;
            InjectionInfo = injectionInfo;
            TypeBeingImported = typeBeingImported;
            MemberName = memberName;
            HasValueProvider = hasValueProvider;
            HasFilter = hasFilter;
            IsSatisfied = isSatisfied;
        }

        /// <summary>
        /// Dependency type 
        /// </summary>
        public DependencyType DependencyType { get; }

        /// <summary>
        /// Activation strategy the dependency belong to
        /// </summary>
        public IActivationStrategy ActivationStrategy { get; set; }

        /// <summary>
        /// The MemberInfo or ParameterInfo that's being injected
        /// </summary>
        public object InjectionInfo { get; }

        /// <summary>
        /// Type being imported
        /// </summary>
        public Type TypeBeingImported { get; }

        /// <summary>
        /// Name of member being injected
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// Has Value Provider
        /// </summary>
        public bool HasValueProvider { get; }

        /// <summary>
        /// Has a filter for it's dependency
        /// </summary>
        public bool HasFilter { get; }

        /// <summary>
        /// Is the dependency satisified
        /// </summary>
        public bool IsSatisfied { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplayString => $"{ReflectionService.GetFriendlyNameForType(TypeBeingImported)} {MemberName} ({DependencyType})";
    }
}
