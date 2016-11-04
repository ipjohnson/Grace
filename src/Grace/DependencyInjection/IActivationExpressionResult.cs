using System.Collections.Generic;
using System.Linq.Expressions;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents an expression and can be used to create a delegate
    /// </summary>
    public interface IActivationExpressionResult
    {
        /// <summary>
        /// Request that generated result
        /// </summary>
        IActivationExpressionRequest Request { get; }

        /// <summary>
        /// Expression for result
        /// </summary>
        Expression Expression { get; set; }

        /// <summary>
        /// Add child expression result
        /// </summary>
        /// <param name="result">expression result</param>
        void AddExpressionResult(IActivationExpressionResult result);
        
        /// <summary>
        /// Add extra parameter for expression 
        /// </summary>
        /// <param name="parameter">parameter to declare</param>
        void AddExtraParameter(ParameterExpression parameter);

        /// <summary>
        /// Extra parameters for result
        /// </summary>
        /// <returns></returns>
        IEnumerable<ParameterExpression> ExtraParameters();

        /// <summary>
        /// Add extra expression to result 
        /// </summary>
        /// <param name="expression">expression for delegate</param>
        void AddExtraExpression(Expression expression);

        /// <summary>
        /// Extra expressions for result
        /// </summary>
        /// <returns></returns>
        IEnumerable<Expression> ExtraExpressions();
    }
}
