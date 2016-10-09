using System.Collections.Generic;
using System.Linq.Expressions;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public class ActivationExpressionResult : IActivationExpressionResult
    {
        private ImmutableLinkedList<ParameterExpression> _parameterExpressions = ImmutableLinkedList<ParameterExpression>.Empty;
        private ImmutableLinkedList<Expression> _extraExpressions = ImmutableLinkedList<Expression>.Empty;

        public ActivationExpressionResult(IActivationExpressionRequest request)
        {
            Request = request;
        }

        public IActivationExpressionRequest Request { get; }

        public Expression Expression { get; set; }

        public void AddExpressionResult(IActivationExpressionResult result)
        {
            _extraExpressions = _extraExpressions.AddRange(result.ExtraExpressions());
            _parameterExpressions = _parameterExpressions.AddRange(result.ExtraParameters());
        }

        public void AddExtraParameter(ParameterExpression parameter)
        {
            _parameterExpressions = _parameterExpressions.Add(parameter);
        }

        public IEnumerable<ParameterExpression> ExtraParameters()
        {
            return _parameterExpressions.Reverse();
        }

        public void AddExtraExpression(Expression expression)
        {
            _extraExpressions = _extraExpressions.Add(expression);
        }

        public IEnumerable<Expression> ExtraExpressions()
        {
            return _extraExpressions.Reverse();
        }
    }
}
