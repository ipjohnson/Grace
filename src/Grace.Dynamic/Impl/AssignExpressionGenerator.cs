using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// Interface for emiting assign expression
    /// </summary>
    public interface IAssignExpressionGenerator
    {
        /// <summary>
        /// Generate IL for assign statement
        /// </summary>
        /// <param name="request"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        bool GenerateIL(DynamicMethodGenerationRequest request, BinaryExpression expression);
    }

    /// <summary>
    /// Expression generator for assign statement
    /// </summary>
    public class AssignExpressionGenerator : IAssignExpressionGenerator
    {
        /// <summary>
        /// Generate IL for assign statement
        /// </summary>
        /// <param name="request"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool GenerateIL(DynamicMethodGenerationRequest request, BinaryExpression expression)
        {
            if (request.TryGenerateIL(request, expression.Right))
            {
                for (int i = 0; i < request.ExtraParameters.Length; i++)
                {
                    if (expression.Left == request.ExtraParameters[i])
                    {
                        switch (i)
                        {
                            case 0:
                                request.ILGenerator.Emit(OpCodes.Stloc_0);
                                return true;
                            case 1:
                                request.ILGenerator.Emit(OpCodes.Stloc_1);
                                return true;
                            case 2:
                                request.ILGenerator.Emit(OpCodes.Stloc_2);
                                return true;
                            case 3:
                                request.ILGenerator.Emit(OpCodes.Stloc_3);
                                return true;
                            default:
                                request.ILGenerator.Emit(OpCodes.Stloc_S, i);
                                return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
