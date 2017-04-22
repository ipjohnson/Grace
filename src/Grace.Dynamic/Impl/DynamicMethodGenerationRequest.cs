using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Grace.DependencyInjection;
using System.Reflection.Emit;

namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// Represents a request to create a dynamic method
    /// </summary>
    public class DynamicMethodGenerationRequest
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="expressionResult"></param>
        /// <param name="tryGenerateIL"></param>
        /// <param name="extraParameters"></param>
        public DynamicMethodGenerationRequest(IActivationExpressionResult expressionResult, Func<DynamicMethodGenerationRequest, Expression, bool> tryGenerateIL, ParameterExpression[] extraParameters)
        {
            ExpressionResult = expressionResult;
            TryGenerateIL = tryGenerateIL;
            ExtraParameters = extraParameters;
            ExpressionRequest = ExpressionResult.Request;
        }

        /// <summary>
        /// Expression result
        /// </summary>
        public IActivationExpressionResult ExpressionResult { get; }

        /// <summary>
        /// Expression Request
        /// </summary>
        public IActivationExpressionRequest ExpressionRequest { get; }

        /// <summary>
        /// Extra parameters for delegate
        /// </summary>
        public ParameterExpression[] ExtraParameters { get; }

        /// <summary>
        /// IL Generator
        /// </summary>
        public ILGenerator ILGenerator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<DynamicMethodGenerationRequest, Expression, bool> TryGenerateIL { get; }

        /// <summary>
        /// List of constants
        /// </summary>
        public List<object> Constants { get; set; }

        /// <summary>
        /// Method target
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// is the constants target an array target
        /// </summary>
        public bool IsArrayTarget { get; set; }
    }
}
