using System.Linq;
using System.Linq.Expressions;

namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// Interface for generating a method call expression
    /// </summary>
    public interface ICallExpressionGenerator
    {
        /// <summary>
        /// Generate IL for method call expression
        /// </summary>
        /// <param name="request">dynamic method request</param>
        /// <param name="expression">expression to convert</param>
        /// <returns></returns>
        bool GenerateIL(DynamicMethodGenerationRequest request, MethodCallExpression expression);
    }

    /// <summary>
    /// class for generating method call expression
    /// </summary>
    public class CallExpressionGenerator : ICallExpressionGenerator
    {
        /// <summary>
        /// Generate IL for method call expression
        /// </summary>
        /// <param name="request">dynamic method request</param>
        /// <param name="expression">expression to convert</param>
        /// <returns></returns>
        public bool GenerateIL(DynamicMethodGenerationRequest request, MethodCallExpression expression)
        {
            if (!request.TryGenerateIL(request, expression.Object))
            {
                return false;
            }

            if (expression.Arguments.Any(expressionArgument => !request.TryGenerateIL(request, expressionArgument)))
            {
                return false;
            }

            request.ILGenerator.EmitMethodCall(expression.Method);

            return true;
        }
    }
}
