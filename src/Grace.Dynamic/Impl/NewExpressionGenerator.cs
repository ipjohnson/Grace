using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// interface for generating IL for new expression
    /// </summary>
    public interface INewExpressionGenerator
    {
        /// <summary>
        /// Generate IL for new expression
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        bool GenerateIL(DynamicMethodGenerationRequest request, NewExpression expression);
    }

    /// <summary>
    /// class for generating IL for new expression
    /// </summary>
    public class NewExpressionGenerator : INewExpressionGenerator
    {
        /// <summary>
        /// Generate IL for new expression
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        public bool GenerateIL(DynamicMethodGenerationRequest request, NewExpression expression)
        {
            if (expression.Arguments.Any(argument => !request.TryGenerateIL(request, argument)))
            {
                return false;
            }

            request.ILGenerator.Emit(OpCodes.Newobj, expression.Constructor);

            return true;
        }
    }
}
