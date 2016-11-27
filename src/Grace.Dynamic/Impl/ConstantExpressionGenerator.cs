using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Grace.Dynamic.Impl
{
    public interface IConstantExpressionGenerator
    {
        bool GenerateIL(DynamicMethodGenerationRequest request, ConstantExpression expression);
    }

    public class ConstantExpressionGenerator : IConstantExpressionGenerator
    {
        public bool GenerateIL(DynamicMethodGenerationRequest request, ConstantExpression expression)
        {
            if (expression.Value == null)
            {
                request.ILGenerator.Emit(OpCodes.Ldnull);

                return true;
            }

            var expressionType = expression.Type;

            if (expressionType == typeof(int))
            {
                request.ILGenerator.EmitInt((int)expression.Value);
            }
            else if (expressionType == typeof(string))
            {
                request.ILGenerator.Emit(OpCodes.Ldstr, (string)expression.Value);
            }
            else if (expressionType == typeof(bool))
            {
                request.ILGenerator.Emit((bool)expression.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            }
            else if (expressionType == typeof(double))
            {
                request.ILGenerator.Emit(OpCodes.Ldc_R8, (double)expression.Value);
            }
            else if (request.IsArrayTarget)
            {
                return false;
            }
            else
            {
                return ProcessConstantFromTarget(request, expression);
            }

            return true;
        }

        private bool ProcessConstantFromTarget(DynamicMethodGenerationRequest request, ConstantExpression expression)
        {
            int constantIndex = request.Constants.IndexOf(expression.Value);

            if (constantIndex < 0)
            {
                return false;
            }

            constantIndex++;

            var field = request.Target.GetType().GetRuntimeField("TArg" + constantIndex);

            request.ILGenerator.Emit(OpCodes.Ldarg_0);

            request.ILGenerator.Emit(OpCodes.Ldfld, field);

            return true;
        }
    }
}
