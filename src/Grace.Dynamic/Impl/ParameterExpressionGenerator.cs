using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Grace.Dynamic.Impl
{
    public interface IParameterExpressionGenerator
    {
        bool GenerateIL(DynamicMethodGenerationRequest request, ParameterExpression expression);
    }

    public class ParameterExpressionGenerator : IParameterExpressionGenerator
    {
        public bool GenerateIL(DynamicMethodGenerationRequest request, ParameterExpression expression)
        {
            var expressionConstants = request.ExpressionRequest.Constants;

            if (expression == expressionConstants.ScopeParameter)
            {
                request.ILGenerator.Emit(OpCodes.Ldarg_1);

                return true;
            }

            if (expression == expressionConstants.RootDisposalScope)
            {
                request.ILGenerator.Emit(OpCodes.Ldarg_2);

                return true;
            }

            if (expression == expressionConstants.InjectionContextParameter)
            {
                request.ILGenerator.Emit(OpCodes.Ldarg_3);

                return true;
            }

            return false;
        }
    }
}
