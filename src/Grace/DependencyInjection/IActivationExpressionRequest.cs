using System;
using System.Linq.Expressions;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
    public enum RequestType
    {
        Root,
        ConstructorParameter,
        Member,
        MethodParameter,
        Other
    }

    public interface IActivationPathNode
    {
        IActivationStrategy Strategy { get; }

        Type ActivationType { get; }

        ICompiledLifestyle Lifestyle { get; }

        IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request);
    }

    public interface IKnownValueExpression
    {
        Type ActivationType { get; }

        IActivationExpressionResult ValueExpression(IActivationExpressionRequest request);
    }

    public interface IDefaultValueInformation
    {
        object DefaultValue { get; }
    }

    public interface IActivationServices
    {
        IAttributeDiscoveryService AttributeDiscoveryService { get; }
        
        IActivationStrategyCompiler Compiler { get; }

        IActivationExpressionBuilder ExpressionBuilder { get; }

        ILifestyleExpressionBuilder ExportExpressionBuilder { get; }

        IInjectionContextCreator InjectionContextCreator { get; }
    }

    public interface IExpressionConstants
    {
        ParameterExpression RootDisposalScope { get; }

        ParameterExpression ScopeParameter { get; }

        ParameterExpression InjectionContextParameter { get; }
    }

    public interface IActivationExpressionRequest
    {
        Type ActivationType { get; }

        object LocateKey { get; }

        Type InjectedType { get; set; }

        RequestType RequestType { get; }

        IActivationExpressionRequest Parent { get; }

        ExportStrategyFilter Filter { get; }

        IActivationServices Services { get; }

        IExpressionConstants Constants { get; }

        bool IsRequired { get; }

        ParameterExpression DisposalScopeExpression { get; set; }

        object Info { get; }

        int ObjectGraphDepth { get; }

        IActivationPathNode DecoratorPathNode { get; }

        IActivationPathNode WrapperPathNode { get; }

        IDefaultValueInformation DefaultValue { get; }

        IActivationExpressionRequest NewRequest(Type activationType, Type injectedType, RequestType requestType, object info, bool maintainPaths = false);

        IActivationPathNode PopWrapperPathNode();

        IActivationPathNode PopDecoratorPathNode();

        void SetDecoratorPath(ImmutableLinkedList<IActivationPathNode> path);

        void SetWrapperPath(ImmutableLinkedList<IActivationPathNode> wrappers);

        void SetIsRequired(bool isRequired);

        void SetLocateKey(object key);

        void SetDefaultValue(IDefaultValueInformation defaultValue);

        ICompiledExportStrategy GetWrappedExportStrategy();

        StaticInjectionContext GetStaticInjectionContext();

        ImmutableLinkedList<InjectionTargetInfo> CreateTargetInfo(ImmutableLinkedList<InjectionTargetInfo> targetInfos);

        ImmutableLinkedList<IKnownValueExpression> KnownValueExpressions { get; }

        void AddKnownValueExpression(IKnownValueExpression knownValueExpression);

        void RequireInjectionContext();

        bool InjectionContextRequired();
    }
}
