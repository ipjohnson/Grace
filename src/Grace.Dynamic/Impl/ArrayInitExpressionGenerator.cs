using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// Interface for generating IL for a NewArray expression
    /// </summary>
    public interface IArrayInitExpressionGenerator
    {
        /// <summary>
        /// Generate IL for new array expression
        /// </summary>
        /// <param name="request">request for generation</param>
        /// <param name="expression">expression to convert</param>
        /// <returns></returns>
        bool GenerateIL(DynamicMethodGenerationRequest request, NewArrayExpression expression);
    }

    /// <summary>
    /// Class for generating IL for a NewArray expression
    /// </summary>
    public class ArrayInitExpressionGenerator : IArrayInitExpressionGenerator
    {
        /// <summary>
        /// Generate IL for new array expression
        /// </summary>
        /// <param name="request">request for generation</param>
        /// <param name="expression">expression to convert</param>
        /// <returns></returns>
        public bool GenerateIL(DynamicMethodGenerationRequest request, NewArrayExpression expression)
        {
            var arrayType = expression.Type;
            var elementType = arrayType.GetElementType();
            var elementIsValueType = elementType.GetTypeInfo().IsValueType;

            var arrayInstance = request.ILGenerator.DeclareLocal(arrayType);

            request.ILGenerator.EmitInt(expression.Expressions.Count);

            request.ILGenerator.Emit(OpCodes.Newarr, elementType);

            request.ILGenerator.Emit(OpCodes.Stloc, arrayInstance);

            var index = 0;

            foreach (var instanceExpression in expression.Expressions)
            {
                request.ILGenerator.Emit(OpCodes.Ldloc, arrayInstance);
                request.ILGenerator.EmitInt(index);

                index++;

                if (elementIsValueType)
                {
                    request.ILGenerator.Emit(OpCodes.Ldelema, elementType);
                }

                if (!request.TryGenerateIL(request, instanceExpression))
                {
                    return false;
                }

                if (elementIsValueType)
                {
                    request.ILGenerator.Emit(OpCodes.Stobj, elementType);
                }
                else
                {
                    request.ILGenerator.Emit(OpCodes.Stelem_Ref);
                }
            }

            request.ILGenerator.Emit(OpCodes.Ldloc, arrayInstance);

            return true;
        }
    }
}
