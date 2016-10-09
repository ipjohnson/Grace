using System;
using System.Linq.Expressions;

namespace Grace.DependencyInjection
{
    public interface IActivationStrategyCompiler
    {
        int MaxObjectGraphDepth { get; }

        IActivationExpressionRequest CreateNewRequest(Type activationType, int objectGraphDepth);

        IActivationExpressionResult CreateNewResult(IActivationExpressionRequest request, Expression expression = null);

        ActivationStrategyDelegate FindDelegate(IInjectionScope scope, Type locateType, object key);
        
        ActivationStrategyDelegate CompileDelegate(IInjectionScope scope, IActivationExpressionResult expressionContext);

        void ProcessMissingStrategyProviders(IInjectionScope scope, IActivationExpressionRequest request);
    }
}
