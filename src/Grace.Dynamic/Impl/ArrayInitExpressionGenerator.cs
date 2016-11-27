using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Grace.Dynamic.Impl
{
    public interface IArrayInitExpressionGenerator
    {
        bool GenerateIL(DynamicMethodGenerationRequest request, NewArrayExpression expression);
    }

    public class ArrayInitExpressionGenerator : IArrayInitExpressionGenerator
    {
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
