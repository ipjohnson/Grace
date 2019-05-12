using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Lifestyle;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Services for request
    /// </summary>
    public class ActivationServices : IActivationServices
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="compiler"></param>
        /// <param name="expressionBuilder"></param>
        /// <param name="attributeDiscoveryService"></param>
        /// <param name="exportExpressionBuilder"></param>
        /// <param name="injectionContextCreator"></param>
        public ActivationServices(IActivationStrategyCompiler compiler,
                                  IActivationExpressionBuilder expressionBuilder,
                                  IAttributeDiscoveryService attributeDiscoveryService,
                                  IDefaultStrategyExpressionBuilder exportExpressionBuilder,
                                  IInjectionContextCreator injectionContextCreator)
        {
            Compiler = compiler;
            ExpressionBuilder = expressionBuilder;
            LifestyleExpressionBuilder = exportExpressionBuilder;
            InjectionContextCreator = injectionContextCreator;
            AttributeDiscoveryService = attributeDiscoveryService;
        }

        /// <summary>
        /// Service for locating attributes
        /// </summary>
        public IAttributeDiscoveryService AttributeDiscoveryService { get; }

        /// <summary>
        /// activation compiler for request
        /// </summary>
        public IActivationStrategyCompiler Compiler { get; }

        /// <summary>
        /// Expression builder that takes lifestyle into consideration
        /// </summary>
        public IDefaultStrategyExpressionBuilder LifestyleExpressionBuilder { get; }

        /// <summary>
        /// Injection context creator
        /// </summary>
        public IInjectionContextCreator InjectionContextCreator { get; }

        /// <summary>
        /// Expression builder
        /// </summary>
        public IActivationExpressionBuilder ExpressionBuilder { get; }
    }

    /// <summary>
    /// Node in decorator path
    /// </summary>
    public class DecoratorActivationPathNode : IActivationPathNode
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="activationType"></param>
        /// <param name="lifestyle"></param>
        public DecoratorActivationPathNode(IDecoratorOrExportActivationStrategy strategy, Type activationType, ICompiledLifestyle lifestyle)
        {
            Strategy = strategy;
            ActivationType = activationType;
            Lifestyle = lifestyle;
        }

        /// <summary>
        /// Strategy to use when activating
        /// </summary>
        public IActivationStrategy Strategy { get; set; }

        /// <summary>
        /// Type the strategy satisfies
        /// </summary>
        public Type ActivationType { get; set; }

        /// <summary>
        /// Lifestyle for activation node
        /// </summary>
        public ICompiledLifestyle Lifestyle { get; set; }

        /// <summary>
        /// Get activation expression
        /// </summary>
        /// <param name="scope">scope for node</param>
        /// <param name="request">request for activation</param>
        /// <returns></returns>
        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            return ((IDecoratorOrExportActivationStrategy)Strategy).GetDecoratorActivationExpression(scope, request, Lifestyle);
        }
    }

    /// <summary>
    /// Node in wrapper path
    /// </summary>
    public class WrapperActivationPathNode : IActivationPathNode
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="activationType"></param>
        /// <param name="lifestyle"></param>
        public WrapperActivationPathNode(IWrapperOrExportActivationStrategy strategy, Type activationType,
            ICompiledLifestyle lifestyle)
        {
            Strategy = strategy;
            ActivationType = activationType;
            Lifestyle = lifestyle;
        }

        /// <summary>
        /// Strategy to use when activating
        /// </summary>
        public IActivationStrategy Strategy { get; set; }

        /// <summary>
        /// Type the strategy satisfies
        /// </summary>
        public Type ActivationType { get; set; }

        /// <summary>
        /// Lifestyle for activation node
        /// </summary>
        public ICompiledLifestyle Lifestyle { get; set; }

        /// <summary>
        /// Get activation expression
        /// </summary>
        /// <param name="scope">scope for node</param>
        /// <param name="request">request for activation</param>
        /// <returns></returns>
        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            return ((IWrapperOrExportActivationStrategy)Strategy).GetActivationExpression(scope, request);
        }
    }

    /// <summary>
    /// Data that is per delegate
    /// </summary>
    public class PerDelegateData : IDataPerDelegate
    {
        private ImmutableHashTree<object, object> _data = ImmutableHashTree<object, object>.Empty;

        /// <summary>
        /// Keys for data
        /// </summary>
        public IEnumerable<object> Keys => _data.Keys;

        /// <summary>
        /// Values for data
        /// </summary>
        public IEnumerable<object> Values => _data.Values;

        /// <summary>
        /// Enumeration of all the key value pairs
        /// </summary>
        public IEnumerable<KeyValuePair<object, object>> KeyValuePairs => _data;

        /// <summary>
        /// Extra data associated with the injection request. 
        /// </summary>
        /// <param name="key">key of the data object to get</param>
        /// <returns>data value</returns>
        public object GetExtraData(object key)
        {
            return _data.GetValueOrDefault(key);
        }

        /// <summary>
        /// Sets extra data on the injection context
        /// </summary>
        /// <param name="key">object name</param>
        /// <param name="newValue">new object value</param>
        /// <param name="replaceIfExists">replace value if key exists</param>
        /// <returns>the final value of key</returns>
        public object SetExtraData(object key, object newValue, bool replaceIfExists = true)
        {
            return ImmutableHashTree.ThreadSafeAdd(ref _data, key, newValue, replaceIfExists);
        }
    }

    /// <summary>
    /// Expression request object
    /// </summary>
    public class ActivationExpressionRequest : IActivationExpressionRequest
    {
        private bool _injectionContextRequired;
        private bool _exportScopeRequired;
        private bool _disposalScopeRequired;
        private ImmutableLinkedList<IActivationPathNode> _wrapperNodes = ImmutableLinkedList<IActivationPathNode>.Empty;
        private ImmutableLinkedList<IActivationPathNode> _decoratorNodes = ImmutableLinkedList<IActivationPathNode>.Empty;
        private ImmutableHashTree<object, object> _extraData = ImmutableHashTree<object, object>.Empty;
        private string _uniqueId;
        private ImmutableLinkedList<InjectionTargetInfo> _targetInfoList;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="requestedType"></param>
        /// <param name="services"></param>
        /// <param name="constants"></param>
        /// <param name="objectGraphDepth"></param>
        /// <param name="requestingScope"></param>
        /// <param name="delegateData"></param>
        public ActivationExpressionRequest(Type activationType, RequestType requestedType, IActivationServices services, IExpressionConstants constants, int objectGraphDepth, IInjectionScope requestingScope, IDataPerDelegate delegateData)
        {
            ActivationType = activationType;

            RequestType = requestedType;
            Services = services;
            Constants = constants;

            KnownValueExpressions = ImmutableLinkedList<IKnownValueExpression>.Empty;
            DisposalScopeExpression = constants.RootDisposalScope;
            ScopeParameter = constants.ScopeParameter;
            InjectionContextParameter = constants.InjectionContextParameter;

            Parent = null;
            IsRequired = true;
            ObjectGraphDepth = objectGraphDepth;
            RequestingScope = requestingScope;
            PerDelegateData = delegateData;
        }

        /// <summary>
        /// Type being requested
        /// </summary>
        public Type ActivationType { get; }

        /// <summary>
        /// Key to use for locating
        /// </summary>
        public object LocateKey { get; set; }

        /// <summary>
        /// Type being injected into
        /// </summary>
        public Type InjectedType { get; private set; }

        /// <summary>
        /// Requesting strategy
        /// </summary>
        public IActivationStrategy RequestingStrategy { get; private set; }

        /// <summary>
        /// Type of request
        /// </summary>
        public RequestType RequestType { get; }

        /// <summary>
        /// Parent request
        /// </summary>
        public IActivationExpressionRequest Parent { get; private set; }

        /// <summary>
        /// Export strategy filter to use
        /// </summary>
        public ActivationStrategyFilter Filter { get; set; }

        /// <summary>
        /// IComparer to be used when locating array or 
        /// </summary>
        public object EnumerableComparer { get; set; }

        /// <summary>
        /// Services for request
        /// </summary>
        public IActivationServices Services { get; }

        /// <summary>
        /// Constants for request
        /// </summary>
        public IExpressionConstants Constants { get; }

        /// <summary>
        /// Is request required
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Disposal scope expression to use
        /// </summary>
        public ParameterExpression DisposalScopeExpression { get; set; }

        /// <summary>
        /// export locator scope parameter
        /// </summary>
        public Expression ScopeParameter { get; set; }

        /// <summary>
        /// Injection context parameter
        /// </summary>
        public Expression InjectionContextParameter { get; set; }

        /// <summary>
        /// Info object for request (MethodInfo, FieldInfo, ParameterInfo)
        /// </summary>
        public object Info { get; private set; }

        /// <summary>
        /// Current object graph depth
        /// </summary>
        public int ObjectGraphDepth { get; }

        /// <summary>
        /// Unique Id for request
        /// </summary>
        public string UniqueId
        {
            get
            {
                if (_uniqueId != null)
                {
                    return _uniqueId;
                }

                return Interlocked.CompareExchange(ref _uniqueId, UniqueStringId.Generate(), null) ?? _uniqueId;
            }
        }

        /// <summary>
        /// Current decorator path if decorating
        /// </summary>
        public IActivationPathNode DecoratorPathNode => _decoratorNodes == ImmutableLinkedList<IActivationPathNode>.Empty ? null : _decoratorNodes.Value;

        /// <summary>
        /// Wrapper path if in the middel of wrapping
        /// </summary>
        public IActivationPathNode WrapperPathNode => _wrapperNodes == ImmutableLinkedList<IActivationPathNode>.Empty ? null : _wrapperNodes.Value;

        /// <summary>
        /// Default value for request if not found
        /// </summary>
        public IDefaultValueInformation DefaultValue { get; private set; }

        /// <summary>
        /// Pop wrapper node off path
        /// </summary>
        /// <returns></returns>
        public IActivationPathNode PopWrapperPathNode()
        {
            if (_wrapperNodes == ImmutableLinkedList<IActivationPathNode>.Empty)
            {
                return null;
            }

            var currentNode = _wrapperNodes;

            _wrapperNodes = currentNode.Next;

            return currentNode.Value;
        }

        /// <summary>
        /// Pop decorator node off path
        /// </summary>
        /// <returns></returns>
        public IActivationPathNode PopDecoratorPathNode()
        {
            if (_decoratorNodes == ImmutableLinkedList<IActivationPathNode>.Empty)
            {
                return null;
            }

            var currentNode = _decoratorNodes;

            _decoratorNodes = currentNode.Next;

            return currentNode.Value;
        }

        /// <summary>
        /// Set filter
        /// </summary>
        /// <param name="filter"></param>
        public void SetFilter(ActivationStrategyFilter filter)
        {
            Filter = filter;
        }

        /// <summary>
        /// Set the comparer for this request
        /// </summary>
        /// <param name="comparer"></param>
        public void SetEnumerableComparer(object comparer)
        {
            EnumerableComparer = comparer;
        }

        /// <summary>
        /// Set the decorator path for request
        /// </summary>
        /// <param name="path">node path</param>
        public void SetDecoratorPath(ImmutableLinkedList<IActivationPathNode> path)
        {
            _decoratorNodes = path;
        }

        /// <summary>
        /// Set wrapper path for request
        /// </summary>
        /// <param name="wrappers">node path</param>
        public void SetWrapperPath(ImmutableLinkedList<IActivationPathNode> wrappers)
        {
            _wrapperNodes = wrappers;
        }

        /// <summary>
        /// Get the currently wrapped strategy if one exists
        /// </summary>
        /// <returns></returns>
        public IActivationStrategy GetWrappedStrategy()
        {
            IActivationStrategy returnStrategy = null;

            _wrapperNodes?.Visit(p =>
            {
                if (p.Strategy.StrategyType == ActivationStrategyType.ExportStrategy)
                {
                    returnStrategy = p.Strategy;
                }
            });

            return returnStrategy;
        }

        /// <summary>
        /// Set is required value for request
        /// </summary>
        /// <param name="isRequired">is value required</param>
        public void SetIsRequired(bool isRequired)
        {
            IsRequired = isRequired;

            if (isRequired)
            {
                Parent?.SetIsRequired(true);
            }
        }

        /// <summary>
        /// Set key for request
        /// </summary>
        /// <param name="key">key to use for request</param>
        public void SetLocateKey(object key)
        {
            LocateKey = key;
        }

        /// <summary>
        /// Set default value for request
        /// </summary>
        /// <param name="defaultValue">default value</param>
        public void SetDefaultValue(IDefaultValueInformation defaultValue)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Get wrapped strategy
        /// </summary>
        /// <returns></returns>
        public ICompiledExportStrategy GetWrappedExportStrategy()
        {
            var pathNode = _wrapperNodes?.LastOrDefault();

            return pathNode?.Strategy as ICompiledExportStrategy;
        }
        
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
        public IActivationExpressionRequest NewRequest(Type activationType, IActivationStrategy requestingStrategy, Type injectedType, RequestType requestType, object info, bool maintainPaths = false, bool carryData = false)
        {
            if (ObjectGraphDepth + 1 > Services.Compiler.MaxObjectGraphDepth)
            {
                throw new RecursiveLocateException(GetStaticInjectionContext());
            }

            var data = carryData ? PerDelegateData : new PerDelegateData();

            var returnValue = new ActivationExpressionRequest(activationType, requestType, Services, Constants, ObjectGraphDepth + 1, RequestingScope, data)
            {
                Parent = this,
                InjectedType = injectedType,
                RequestingStrategy = requestingStrategy,
                Info = info,
                DisposalScopeExpression = DisposalScopeExpression,
                ScopeParameter = ScopeParameter,
                InjectionContextParameter = InjectionContextParameter,
                KnownValueExpressions = KnownValueExpressions
            };

            if (Filter != null && RequestingStrategy != null &&
                RequestingStrategy.StrategyType == ActivationStrategyType.WrapperStrategy)
            {
                returnValue.Filter = Filter;
            }

            if (maintainPaths)
            {
                returnValue._wrapperNodes = _wrapperNodes;
                returnValue._decoratorNodes = _decoratorNodes;
            }

            return returnValue;
        }

        /// <summary>
        /// Creates new rooted request (for lifestyles)
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="requestingScope"></param>
        /// <param name="maintainPaths"></param>
        /// <returns></returns>
        public IActivationExpressionRequest NewRootedRequest(Type activationType, IInjectionScope requestingScope,
            bool maintainPaths = false)
        {
            if (ObjectGraphDepth + 1 > Services.Compiler.MaxObjectGraphDepth)
            {
                throw new RecursiveLocateException(GetStaticInjectionContext());
            }

            var returnValue = new ActivationExpressionRequest(activationType,
                                                   RequestType.Root,
                                                   Services,
                                                   Constants,
                                                   ObjectGraphDepth + 1,
                                                   requestingScope,
                                                   new PerDelegateData());

            if (maintainPaths)
            {
                returnValue._wrapperNodes = _wrapperNodes;
                returnValue._decoratorNodes = _decoratorNodes;
            }

            return returnValue;
        }

        /// <summary>
        /// Original requesting scope
        /// </summary>
        public IInjectionScope RequestingScope { get; }

        /// <summary>
        /// Data that is per delegate and won't transfer to other delegates
        /// </summary>
        public IDataPerDelegate PerDelegateData { get; }

        /// <summary>
        /// Get static injection context for request
        /// </summary>
        /// <returns></returns>
        public StaticInjectionContext GetStaticInjectionContext()
        {
            var currentTargetInfo = CreateTargetInfo();

            return new StaticInjectionContext(ActivationType, currentTargetInfo);
        }

        /// <summary>
        /// Create target info for request
        /// </summary>
        /// <returns></returns>
        public ImmutableLinkedList<InjectionTargetInfo> CreateTargetInfo()
        {
            if (_targetInfoList != null)
            {
                return _targetInfoList;
            }

            var targetName = "";

            if (Info is ParameterInfo info)
            {
                targetName = info.Name;
            }
            else if (Info is MemberInfo memberInfo)
            {
                targetName = memberInfo.Name;
            }

            var targetInfo =
                new InjectionTargetInfo(Services.AttributeDiscoveryService, InjectedType, RequestingStrategy, Info, targetName, RequestType, ActivationType, false, null, UniqueId);

            _targetInfoList = Parent?.CreateTargetInfo() ?? ImmutableLinkedList<InjectionTargetInfo>.Empty;

            _targetInfoList = _targetInfoList.Add(targetInfo);

            return _targetInfoList;
        }

        /// <summary>
        /// Known values that can be used in request
        /// </summary>
        public ImmutableLinkedList<IKnownValueExpression> KnownValueExpressions { get; private set; }

        /// <summary>
        /// Is the request dynamic
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// Add known value expression to request
        /// </summary>
        /// <param name="knownValueExpression">known value expression</param>
        public void AddKnownValueExpression(IKnownValueExpression knownValueExpression)
        {
            KnownValueExpressions = KnownValueExpressions.Add(knownValueExpression);
        }

        /// <summary>
        /// Require injection context for request
        /// </summary>
        public void RequireInjectionContext()
        {
            if (!_injectionContextRequired)
            {
                _injectionContextRequired = true;

                Parent?.RequireInjectionContext();
            }
        }

        /// <summary>
        /// Is injection context required
        /// </summary>
        /// <returns></returns>
        public bool InjectionContextRequired()
        {
            return _injectionContextRequired;
        }

        /// <summary>
        /// Require export scope
        /// </summary>
        public void RequireExportScope()
        {
            if (!_exportScopeRequired)
            {
                _exportScopeRequired = true;
                Parent?.RequireExportScope();
            }
        }

        /// <summary>
        /// Is export scope required
        /// </summary>
        /// <returns></returns>
        public bool ExportScopeRequired()
        {
            return _exportScopeRequired;
        }

        /// <summary>
        /// Require disposal scope
        /// </summary>
        public void RequireDisposalScope()
        {
            if (!_disposalScopeRequired)
            {
                _disposalScopeRequired = true;
                Parent?.RequireDisposalScope();
            }
        }

        /// <summary>
        /// Disposal scope is required
        /// </summary>
        /// <returns></returns>
        public bool DisposalScopeRequired()
        {
            return _disposalScopeRequired;
        }

        /// <summary>
        /// Keys for data
        /// </summary>
        public IEnumerable<object> Keys => _extraData.Keys;

        /// <summary>
        /// Values for data
        /// </summary>
        public IEnumerable<object> Values => _extraData.Values;

        /// <summary>
        /// Enumeration of all the key value pairs
        /// </summary>
        public IEnumerable<KeyValuePair<object, object>> KeyValuePairs => _extraData;

        /// <summary>
        /// Extra data associated with the injection request. 
        /// </summary>
        /// <param name="key">key of the data object to get</param>
        /// <returns>data value</returns>
        public object GetExtraData(object key)
        {
            return _extraData.GetValueOrDefault(key);
        }

        /// <summary>
        /// Sets extra data on the injection context
        /// </summary>
        /// <param name="key">object name</param>
        /// <param name="newValue">new object value</param>
        /// <param name="replaceIfExists">replace value if key exists</param>
        /// <returns>the final value of key</returns>
        public object SetExtraData(object key, object newValue, bool replaceIfExists = true)
        {
            return ImmutableHashTree.ThreadSafeAdd(ref _extraData, key, newValue, replaceIfExists);
        }
    }
}
