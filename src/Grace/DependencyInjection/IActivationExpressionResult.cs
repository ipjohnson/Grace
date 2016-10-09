using System.Collections.Generic;
using System.Linq.Expressions;

namespace Grace.DependencyInjection
{
    public interface IActivationExpressionResult
    {
        IActivationExpressionRequest Request { get; }

        Expression Expression { get; set; }

        void AddExpressionResult(IActivationExpressionResult result);

        void AddExtraParameter(ParameterExpression parameter);

        IEnumerable<ParameterExpression> ExtraParameters();

        void AddExtraExpression(Expression expression);

        IEnumerable<Expression> ExtraExpressions();
    }
}
