﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// Interface for generating IL for a MemeberInitExpression
    /// </summary>
    public interface IMemeberInitExpressionGenerator
    {
        /// <summary>
        /// Generate IL for member init expression
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        bool GenerateIL(DynamicMethodGenerationRequest request, MemberInitExpression expression);
    }

    /// <summary>
    /// Class for generating IL for MemberInit expression
    /// </summary>
    public class MemeberInitExpressionGenerator : IMemeberInitExpressionGenerator
    {
        /// <summary>
        /// Generate IL for member init expression
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        public bool GenerateIL(DynamicMethodGenerationRequest request, MemberInitExpression expression)
        {
            if (!request.TryGenerateIL(request, expression.NewExpression))
            {
                return false;
            }

            var instance = request.ILGenerator.DeclareLocal(expression.Type);

            request.ILGenerator.Emit(OpCodes.Stloc, instance);

            foreach (var binding in expression.Bindings)
            {
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    return false;
                }

                request.ILGenerator.Emit(OpCodes.Ldloc, instance);

                if (!request.TryGenerateIL(request, ((MemberAssignment) binding).Expression))
                {
                    return false;
                }

                var propertyInfo = binding.Member as PropertyInfo;

                if (propertyInfo != null)
                {
                    var setMethod = propertyInfo.SetMethod;

                    if (setMethod == null)
                    {
                        return false;
                    }

                    request.ILGenerator.EmitMethodCall(setMethod);
                }
                else if (binding.Member is FieldInfo)
                {
                    request.ILGenerator.Emit(OpCodes.Stfld, (FieldInfo)binding.Member);
                }
                else
                {
                    return false;
                }
            }

            request.ILGenerator.Emit(OpCodes.Ldloc, instance);

            return true;
        }
    }
}