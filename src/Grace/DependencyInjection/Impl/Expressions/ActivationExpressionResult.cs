using System.Collections.Generic;
using System.Linq.Expressions;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Expression result
    /// </summary>
    public class ActivationExpressionResult : IActivationExpressionResult
    {
        private ImmutableLinkedList<ParameterExpression> _parameterExpressions = ImmutableLinkedList<ParameterExpression>.Empty;
        private ImmutableLinkedList<Expression> _extraExpressions = ImmutableLinkedList<Expression>.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="request"></param>
        public ActivationExpressionResult(IActivationExpressionRequest request)
        {
            Request = request;
        }

        /// <summary>
        /// Request that generated result
        /// </summary>
        public IActivationExpressionRequest Request { get; }

        /// <summary>
        /// Expression for result
        /// </summary>
        public Expression Expression { get; set; }

        /// <summary>
        /// True if no type was found and the default fallback expression was used
        /// </summary>
        public bool UsingFallbackExpression { get; set; } = false;

        /// <summary>
        /// Add child expression result
        /// </summary>
        /// <param name="result">expression result</param>
        public void AddExpressionResult(IActivationExpressionResult result)
        {
            _extraExpressions = _extraExpressions.AddRange(result.ExtraExpressions());
            _parameterExpressions = _parameterExpressions.AddRange(result.ExtraParameters());
        }

        /// <summary>
        /// Add extra parameter for expression 
        /// </summary>
        /// <param name="parameter">parameter to declare</param>
        public void AddExtraParameter(ParameterExpression parameter)
        {
            _parameterExpressions = _parameterExpressions.Add(parameter);
        }

        /// <summary>
        /// Extra parameters for result
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ParameterExpression> ExtraParameters()
        {
            return _parameterExpressions.Reverse();
        }

        /// <summary>
        /// Add extra expression to result 
        /// </summary>
        /// <param name="expression">expression for delegate</param>
        /// <param name="insertBeginning"></param>
        public void AddExtraExpression(Expression expression, bool insertBeginning = false)
        {
            if (insertBeginning)
            {
                var expressions = _extraExpressions;

                _extraExpressions = ImmutableLinkedList<Expression>.Empty.Add(expression);

                expressions.Visit(e => _extraExpressions = _extraExpressions.Add(e), startAtEnd: true);
            }
            else
            {
                _extraExpressions = _extraExpressions.Add(expression);
            }
        }

        /// <summary>
        /// Extra expressions for result
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Expression> ExtraExpressions()
        {
            return _extraExpressions.Reverse();
        }
    }
}
