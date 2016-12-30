using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// interface for generating IL for a constant expression
    /// </summary>
    public interface IConstantExpressionGenerator
    {
        /// <summary>
        /// Generate IL for a constant expression
        /// </summary>
        /// <param name="request">dynamic request</param>
        /// <param name="expression">constant expression to convert</param>
        /// <returns></returns>
        bool GenerateIL(DynamicMethodGenerationRequest request, ConstantExpression expression);
    }

    /// <summary>
    /// Class for converting constant expression to IL
    /// </summary>
    public class ConstantExpressionGenerator : IConstantExpressionGenerator
    {
        /// <summary>
        /// Generate IL for a constant expression
        /// </summary>
        /// <param name="request">dynamic request</param>
        /// <param name="expression">constant expression to convert</param>
        /// <returns></returns>
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
                return ProcessConstantFromArrayTarget(request, expression);
            }
            else
            {
                return ProcessConstantFromTarget(request, expression);
            }

            return true;
        }

        private bool ProcessConstantFromArrayTarget(DynamicMethodGenerationRequest request, ConstantExpression expression)
        {

            var constantIndex = request.Constants.IndexOf(expression.Value);

            if (constantIndex < 0)
            {
                return false;
            }

            var field = request.Target.GetType().GetRuntimeField("Items");

            request.ILGenerator.Emit(OpCodes.Ldarg_0);

            request.ILGenerator.Emit(OpCodes.Ldfld, field);

            request.ILGenerator.EmitInt(constantIndex);

            request.ILGenerator.Emit(OpCodes.Ldelem_Ref);

            if (expression.Type != typeof(object))
            {
                request.ILGenerator.Emit(OpCodes.Castclass, expression.Type);
            }

            return true;
        }

        private bool ProcessConstantFromTarget(DynamicMethodGenerationRequest request, ConstantExpression expression)
        {
            var constantIndex = request.Constants.IndexOf(expression.Value);

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
