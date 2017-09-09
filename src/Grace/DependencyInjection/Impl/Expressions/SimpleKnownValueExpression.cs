using System;
using System.Linq.Expressions;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Represents a known value as a linq expression
    /// </summary>
    public class SimpleKnownValueExpression : IKnownValueExpression
    {
        private readonly Expression _expression;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="expression"></param>
        /// <param name="key"></param>
        /// <param name="position"></param>
        public SimpleKnownValueExpression(Type activationType, Expression expression, object key = null, int? position = null)
        {
            ActivationType = activationType;
            _expression = expression;
            Key = key;
            Position = position;
        }

        /// <summary>
        /// Type for expression
        /// </summary>
        public Type ActivationType { get; }

        /// <summary>
        /// Hint for locating when multiple
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// Hint for where the value should be positioned
        /// </summary>
        public int? Position { get; }

        /// <summary>
        /// Expression that represents the known value
        /// </summary>
        /// <param name="request">request for expression</param>
        /// <returns></returns>
        public IActivationExpressionResult ValueExpression(IActivationExpressionRequest request)
        {
            return request.Services.Compiler.CreateNewResult(request, _expression);
        }
    }
}
