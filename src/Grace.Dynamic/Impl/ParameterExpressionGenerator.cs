using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// interface for generating IL for Parameter expression
    /// </summary>
    public interface IParameterExpressionGenerator
    {
        /// <summary>
        /// Generate IL for ParameterExpression
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        bool GenerateIL(DynamicMethodGenerationRequest request, ParameterExpression expression);
    }

    /// <summary>
    /// class for generating parameter expression
    /// </summary>
    public class ParameterExpressionGenerator : IParameterExpressionGenerator
    {
        /// <summary>
        /// Generate IL for ParameterExpression
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="expression">expression</param>
        /// <returns></returns>
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
