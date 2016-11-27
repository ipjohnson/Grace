using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Grace.Dynamic.Impl
{
    public interface ICallExpressionGenerator
    {
        bool GenerateIL(DynamicMethodGenerationRequest request, MethodCallExpression expression);
    }

    public class CallExpressionGenerator : ICallExpressionGenerator
    {
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
