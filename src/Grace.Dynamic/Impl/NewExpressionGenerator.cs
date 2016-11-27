using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Grace.Dynamic.Impl
{
    public interface INewExpressionGenerator
    {
        bool GenerateIL(DynamicMethodGenerationRequest request, NewExpression expression);
    }

    public class NewExpressionGenerator : INewExpressionGenerator
    {
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
