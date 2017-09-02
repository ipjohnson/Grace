using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Replaces Arg.Locate expressions with expressions.
    /// </summary>
    public class LocateExpressionReplacer : ExpressionVisitor
    {
        private readonly IActivationExpressionRequest _request;
        private readonly IActivationStrategy _activationStrategy;
        private IActivationExpressionResult _result;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="activationStrategy"></param>
        public LocateExpressionReplacer(IActivationExpressionRequest request, IActivationStrategy activationStrategy)
        {
            _request = request;
            _activationStrategy = activationStrategy;
        }

        /// <summary>
        /// Replace values in expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IActivationExpressionResult Replace(Expression expression)
        {
            _result = _request.Services.Compiler.CreateNewResult(_request);

            _result.Expression = Visit(expression);

            return _result;
        }

        /// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.</summary>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        /// <param name="node">The expression to visit.</param>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Arg))
            {
                var methodName = node.Method.Name;

                if (methodName == nameof(Arg.Any) ||
                    methodName == nameof(Arg.Locate))
                {
                    var newRequest = _request.NewRequest(node.Method.GetGenericArguments().First(), _activationStrategy,
                        _activationStrategy.ActivationType, RequestType.Other, null, true, true);

                    var activationExpression =
                        _request.Services.ExpressionBuilder.GetActivationExpression(_request.RequestingScope, newRequest);

                    _result.AddExpressionResult(activationExpression);

                    return activationExpression.Expression;
                }
            }

            return base.VisitMethodCall(node);
        }
    }
}
