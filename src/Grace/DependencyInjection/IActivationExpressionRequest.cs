using System;
using System.Linq.Expressions;
using Grace.Data;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Type of request (Root, Constructor parameter, etc)
    /// </summary>
    public enum RequestType
    {
        /// <summary>
        /// Root of the object graph
        /// </summary>
        Root,

        /// <summary>
        /// for constructor parameter
        /// </summary>
        ConstructorParameter,

        /// <summary>
        /// For member (field or property)
        /// </summary>
        Member,

        /// <summary>
        /// Method parameter
        /// </summary>
        MethodParameter,

        /// <summary>
        /// Unknown
        /// </summary>
        Other
    }

    /// <summary>
    /// Activation node for wrapper or decorator path
    /// </summary>
    public interface IActivationPathNode
    {
        /// <summary>
        /// Strategy to use when activating
        /// </summary>
        IActivationStrategy Strategy { get; }

        /// <summary>
        /// Type the strategy satisfies
        /// </summary>
        Type ActivationType { get; }

        /// <summary>
        /// Lifestyle for activation node
        /// </summary>
        ICompiledLifestyle Lifestyle { get; }

        /// <summary>
        /// Get activation expression
        /// </summary>
        /// <param name="scope">scope for node</param>
        /// <param name="request">request for activation</param>
        /// <returns></returns>
        IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request);
    }

    /// <summary>
    /// Known value expression
    /// </summary>
    public interface IKnownValueExpression
    {
        /// <summary>
        /// Type for expression
        /// </summary>
        Type ActivationType { get; }

        /// <summary>
        /// Hint for locating when multiple
        /// </summary>
        object Key { get; }

        /// <summary>
        /// Hint for where the value should be positioned
        /// </summary>
        int? Position { get; }

        /// <summary>
        /// Expression that represents the known value
        /// </summary>
        /// <param name="request">request for expression</param>
        /// <returns></returns>
        IActivationExpressionResult ValueExpression(IActivationExpressionRequest request);
    }

    /// <summary>
    /// Default value information
    /// </summary>
    public interface IDefaultValueInformation
    {
        /// <summary>
        /// Default value
        /// </summary>
        object DefaultValue { get; }
    }

    /// <summary>
    /// Services to be used during request
    /// </summary>
    public interface IActivationServices
    {
        /// <summary>
        /// Service for locating attributes
        /// </summary>
        IAttributeDiscoveryService AttributeDiscoveryService { get; }
        
        /// <summary>
        /// Service for compiling delegates
        /// </summary>
        IActivationStrategyCompiler Compiler { get; }

        /// <summary>
        /// Linq expression builder
        /// </summary>
        IActivationExpressionBuilder ExpressionBuilder { get; }

        /// <summary>
        /// Expression builder that takes lifestyle into consideration
        /// </summary>
        IDefaultStrategyExpressionBuilder LifestyleExpressionBuilder { get; }

        /// <summary>
        /// Injection context creator
        /// </summary>
        IInjectionContextCreator InjectionContextCreator { get; }
    }

    /// <summary>
    /// Expression constants for request
    /// </summary>
    public interface IExpressionConstants
    {
        /// <summary>
        /// Root disposal scope
        /// </summary>
        ParameterExpression RootDisposalScope { get; }

        /// <summary>
        /// export locator scope parameter
        /// </summary>
        ParameterExpression ScopeParameter { get; }

        /// <summary>
        /// Injection context parameter
        /// </summary>
        ParameterExpression InjectionContextParameter { get; }
    }

    /// <summary>
    /// Data that is per delegate, used for lifestyles
    /// </summary>
    public interface IDataPerDelegate : IExtraDataContainer
    {
        
    }

    /// <summary>
    /// Request context to create expression
    /// </summary>
    public interface IActivationExpressionRequest : IExtraDataContainer
    {
        /// <summary>
        /// Type being requested
        /// </summary>
        Type ActivationType { get; }

        /// <summary>
        /// Key to use for locating
        /// </summary>
        object LocateKey { get; }

        /// <summary>
        /// Type being injected into
        /// </summary>
        Type InjectedType { get; }

        /// <summary>
        /// Requesting strategy
        /// </summary>
        IActivationStrategy RequestingStrategy { get; }

        /// <summary>
        /// Type of request
        /// </summary>
        RequestType RequestType { get; }

        /// <summary>
        /// Parent request
        /// </summary>
        IActivationExpressionRequest Parent { get; }

        /// <summary>
        /// Export strategy filter to use
        /// </summary>
        ActivationStrategyFilter Filter { get; }

        /// <summary>
        /// IComparer to be used when locating array or 
        /// </summary>
        object EnumerableComparer { get; }

        /// <summary>
        /// Services for request
        /// </summary>
        IActivationServices Services { get; }

        /// <summary>
        /// Constants for request
        /// </summary>
        IExpressionConstants Constants { get; }

        /// <summary>
        /// Is request required
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        /// Disposal scope expression to use
        /// </summary>
        ParameterExpression DisposalScopeExpression { get; set; }

        /// <summary>
        /// export locator scope parameter
        /// </summary>
        Expression ScopeParameter { get; set; }

        /// <summary>
        /// Injection context parameter
        /// </summary>
        Expression InjectionContextParameter { get; set; }

        /// <summary>
        /// Info object for request (MethodInfo, FieldInfo, ParameterInfo)
        /// </summary>
        object Info { get; }

        /// <summary>
        /// Current object graph depth
        /// </summary>
        int ObjectGraphDepth { get; }

        /// <summary>
        /// Current decorator path if decorating
        /// </summary>
        IActivationPathNode DecoratorPathNode { get; }

        /// <summary>
        /// Wrapper path if in the middel of wrapping
        /// </summary>
        IActivationPathNode WrapperPathNode { get; }

        /// <summary>
        /// Default value for request if not found
        /// </summary>
        IDefaultValueInformation DefaultValue { get; }

        /// <summary>
        /// Is the request dynamic
        /// </summary>
        bool IsDynamic { get; set; }
        
        /// <summary>
        /// Known values that can be used in request
        /// </summary>
        ImmutableLinkedList<IKnownValueExpression> KnownValueExpressions { get; }

        /// <summary>
        /// Pop wrapper node off path
        /// </summary>
        /// <returns></returns>
        IActivationPathNode PopWrapperPathNode();

        /// <summary>
        /// Pop decorator node off path
        /// </summary>
        /// <returns></returns>
        IActivationPathNode PopDecoratorPathNode();

        /// <summary>
        /// Set filter for this request
        /// </summary>
        /// <param name="filter"></param>
        void SetFilter(ActivationStrategyFilter filter);

        /// <summary>
        /// Set the comparer for this request
        /// </summary>
        /// <param name="comparer"></param>
        void SetEnumerableComparer(object comparer);

        /// <summary>
        /// Set the decorator path for request
        /// </summary>
        /// <param name="path">node path</param>
        void SetDecoratorPath(ImmutableLinkedList<IActivationPathNode> path);

        /// <summary>
        /// Set wrapper path for request
        /// </summary>
        /// <param name="wrappers">node path</param>
        void SetWrapperPath(ImmutableLinkedList<IActivationPathNode> wrappers);

        /// <summary>
        /// Get the currently wrapped strategy if one exists
        /// </summary>
        /// <returns></returns>
        IActivationStrategy GetWrappedStrategy();

        /// <summary>
        /// Set is required value for request
        /// </summary>
        /// <param name="isRequired">is value required</param>
        void SetIsRequired(bool isRequired);

        /// <summary>
        /// Set key for request
        /// </summary>
        /// <param name="key">key to use for request</param>
        void SetLocateKey(object key);

        /// <summary>
        /// Set default value for request
        /// </summary>
        /// <param name="defaultValue">default value</param>
        void SetDefaultValue(IDefaultValueInformation defaultValue);

        /// <summary>
        /// Get wrapped strategy
        /// </summary>
        /// <returns></returns>
        ICompiledExportStrategy GetWrappedExportStrategy();

        /// <summary>
        /// Get static injection context for request
        /// </summary>
        /// <returns></returns>
        StaticInjectionContext GetStaticInjectionContext();

        /// <summary>
        /// Create target info for request
        /// </summary>
        /// <returns></returns>
        ImmutableLinkedList<InjectionTargetInfo> CreateTargetInfo();
        
        /// <summary>
        /// Add known value expression to request
        /// </summary>
        /// <param name="knownValueExpression">known value expression</param>
        void AddKnownValueExpression(IKnownValueExpression knownValueExpression);

        /// <summary>
        /// Require injection context for request
        /// </summary>
        void RequireInjectionContext();

        /// <summary>
        /// Is injection context required
        /// </summary>
        /// <returns></returns>
        bool InjectionContextRequired();

        /// <summary>
        /// Require export scope for resolution
        /// </summary>
        void RequireExportScope();

        /// <summary>
        /// Export scope is required
        /// </summary>
        /// <returns></returns>
        bool ExportScopeRequired();

        /// <summary>
        /// Require disposal scope 
        /// </summary>
        void RequireDisposalScope();

        /// <summary>
        /// Disposal scope required
        /// </summary>
        /// <returns></returns>
        bool DisposalScopeRequired();

        /// <summary>
        /// Create new request from this request
        /// </summary>
        /// <param name="activationType">request type</param>
        /// <param name="requestingStrategy">requesting strategy</param>
        /// <param name="injectedType">type being injected into</param>
        /// <param name="requestType">request type</param>
        /// <param name="info">info for request</param>
        /// <param name="maintainPaths">maintain wrapper and decorator path</param>
        /// <param name="carryData"></param>
        /// <returns>new request</returns>
        IActivationExpressionRequest NewRequest(Type activationType, IActivationStrategy requestingStrategy, Type injectedType, RequestType requestType, object info, bool maintainPaths = false, bool carryData = false);

        /// <summary>
        /// Creates new rooted request (for lifestyles)
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="requestingScope"></param>
        /// <param name="maintainPaths"></param>
        /// <returns></returns>
        IActivationExpressionRequest NewRootedRequest(Type activationType, IInjectionScope requestingScope, bool maintainPaths = false);

        /// <summary>
        /// Scope the request originated in
        /// </summary>
        IInjectionScope RequestingScope { get; }

        /// <summary>
        /// Data that is per delegate and won't transfer to other delegates
        /// </summary>
        IDataPerDelegate PerDelegateData { get; }
    }
}
