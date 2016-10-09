using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public class ActivationServices : IActivationServices
    {
        public ActivationServices(IActivationStrategyCompiler compiler,
                                  IActivationExpressionBuilder expressionBuilder,
                                  IAttributeDiscoveryService attributeDiscoveryService,
                                  ILifestyleExpressionBuilder exportExpressionBuilder,
                                  IInjectionContextCreator injectionContextCreator)
        {
            Compiler = compiler;
            ExpressionBuilder = expressionBuilder;
            ExportExpressionBuilder = exportExpressionBuilder;
            InjectionContextCreator = injectionContextCreator;
            AttributeDiscoveryService = attributeDiscoveryService;
        }

        public IAttributeDiscoveryService AttributeDiscoveryService { get; }

        /// <summary>
        /// activation compiler for request
        /// </summary>
        public IActivationStrategyCompiler Compiler { get; }

        public ILifestyleExpressionBuilder ExportExpressionBuilder { get; }

        public IInjectionContextCreator InjectionContextCreator { get; }

        /// <summary>
        /// Expression builder
        /// </summary>
        public IActivationExpressionBuilder ExpressionBuilder { get; }
    }

    public class DecoratorActivationPathNode : IActivationPathNode
    {
        public DecoratorActivationPathNode(IDecoratorOrExportActivationStrategy strategy, Type activationType, ICompiledLifestyle lifestyle)
        {
            Strategy = strategy;
            ActivationType = activationType;
            Lifestyle = lifestyle;
        }

        public IActivationStrategy Strategy { get; set; }

        public Type ActivationType { get; set; }

        public ICompiledLifestyle Lifestyle { get; set; }

        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            return ((IDecoratorOrExportActivationStrategy)Strategy).GetDecoratorActivationExpression(scope, request, Lifestyle);
        }
    }

    public class WrapperActivationPathNode : IActivationPathNode
    {
        public WrapperActivationPathNode(IWrapperOrExportActivationStrategy strategy, Type activationType,
            ICompiledLifestyle lifestyle)
        {
            Strategy = strategy;
            ActivationType = activationType;
            Lifestyle = lifestyle;
        }

        public IActivationStrategy Strategy { get; set; }

        public Type ActivationType { get; set; }

        public ICompiledLifestyle Lifestyle { get; set; }

        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            return ((IWrapperOrExportActivationStrategy)Strategy).GetActivationExpression(scope, request);
        }
    }

    public class ActivationExpressionRequest : IActivationExpressionRequest
    {
        private bool _injectionContextRequired;
        private ImmutableLinkedList<IActivationPathNode> _wrapperNodes = ImmutableLinkedList<IActivationPathNode>.Empty;
        private ImmutableLinkedList<IActivationPathNode> _decoratorNodes = ImmutableLinkedList<IActivationPathNode>.Empty;
        private string _uniqueId;

        public ActivationExpressionRequest(Type activationType, RequestType requestedType, IActivationServices services, IExpressionConstants constants, int objectGraphDepth)
        {
            ActivationType = activationType;

            RequestType = requestedType;
            Services = services;
            Constants = constants;

            KnownValueExpressions = ImmutableLinkedList<IKnownValueExpression>.Empty;
            DisposalScopeExpression = constants.RootDisposalScope;

            Parent = null;
            IsRequired = true;
            ObjectGraphDepth = objectGraphDepth;
        }

        public Type ActivationType { get; }

        public object LocateKey { get; set; }

        public Type InjectedType { get; set; }

        public RequestType RequestType { get; }

        public IActivationExpressionRequest Parent { get; private set; }

        public ExportStrategyFilter Filter { get; set; }

        public IActivationServices Services { get; }

        public IExpressionConstants Constants { get; }

        public bool IsRequired { get; private set; }

        public ParameterExpression DisposalScopeExpression { get; set; }

        public object Info { get; private set; }

        public int ObjectGraphDepth { get; }

        public string UniqueId
        {
            get
            {
                if (_uniqueId != null)
                {
                    return _uniqueId;
                }

                return Interlocked.CompareExchange(ref _uniqueId, Guid.NewGuid().ToString(), null) ?? _uniqueId;
            }
        }

        public IActivationPathNode DecoratorPathNode => _decoratorNodes == ImmutableLinkedList<IActivationPathNode>.Empty ? null : _decoratorNodes.Value;

        public IActivationPathNode WrapperPathNode => _wrapperNodes == ImmutableLinkedList<IActivationPathNode>.Empty ? null : _wrapperNodes.Value;

        public IDefaultValueInformation DefaultValue { get; private set; }

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

        public void SetDecoratorPath(ImmutableLinkedList<IActivationPathNode> path)
        {
            _decoratorNodes = path;
        }

        public void SetWrapperPath(ImmutableLinkedList<IActivationPathNode> wrappers)
        {
            _wrapperNodes = wrappers;
        }

        public void SetIsRequired(bool isRequired)
        {
            IsRequired = isRequired;

            if (isRequired)
            {
                Parent?.SetIsRequired(true);
            }
        }

        public void SetLocateKey(object key)
        {
            LocateKey = key;
        }

        public void SetDefaultValue(IDefaultValueInformation defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public ICompiledExportStrategy GetWrappedExportStrategy()
        {
            var pathNode = _wrapperNodes?.LastOrDefault();

            return pathNode?.Strategy as ICompiledExportStrategy;
        }

        public IActivationExpressionRequest NewRequest(Type activationType, Type injectedType, RequestType requestType, object info, bool followPath = false)
        {
            if (ObjectGraphDepth + 1 > Services.Compiler.MaxObjectGraphDepth)
            {
                throw new RecursiveLocateException(GetStaticInjectionContext());
            }

            var returnValue = new ActivationExpressionRequest(activationType, requestType, Services, Constants, ObjectGraphDepth + 1)
            {
                Parent = this,
                InjectedType = injectedType,
                Info = info,
                DisposalScopeExpression = DisposalScopeExpression,
                KnownValueExpressions = KnownValueExpressions
            };

            if (followPath)
            {
                returnValue._wrapperNodes = _wrapperNodes;
                returnValue._decoratorNodes = _decoratorNodes;
            }

            return returnValue;
        }

        public StaticInjectionContext GetStaticInjectionContext()
        {
            var currentTargetInfo = CreateTargetInfo(ImmutableLinkedList<InjectionTargetInfo>.Empty);

            return new StaticInjectionContext(ActivationType, currentTargetInfo);
        }

        public ImmutableLinkedList<InjectionTargetInfo> CreateTargetInfo(ImmutableLinkedList<InjectionTargetInfo> targetInfos)
        {
            targetInfos = Parent?.CreateTargetInfo(targetInfos) ?? targetInfos;

            return targetInfos.Add(new InjectionTargetInfo(Services.AttributeDiscoveryService, InjectedType, Info, RequestType, ActivationType, false, null, UniqueId));
        }

        public ImmutableLinkedList<IKnownValueExpression> KnownValueExpressions { get; private set; }

        public void AddKnownValueExpression(IKnownValueExpression knownValueExpression)
        {
            KnownValueExpressions = KnownValueExpressions.Add(knownValueExpression);
        }

        public void RequireInjectionContext()
        {
            _injectionContextRequired = true;

            Parent?.RequireInjectionContext();
        }

        public bool InjectionContextRequired()
        {
            return _injectionContextRequired;
        }
    }
}
