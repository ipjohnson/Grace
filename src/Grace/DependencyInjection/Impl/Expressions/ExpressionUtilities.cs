using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Expression utilities
    /// </summary>
    public static class ExpressionUtilities
    {
        #region CreateExpressionsForTypes
        /// <summary>
        /// Create an array of expressions based off an array of types
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="resultType"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IActivationExpressionResult[] CreateExpressionsForTypes(IActivationStrategy strategy, IInjectionScope scope,
            IActivationExpressionRequest request, Type resultType, params Type[] types)
        {
            var resultArray = new IActivationExpressionResult[types.Length];

            for (var i = 0; i < types.Length; i++)
            {
                var arg1Request = request.NewRequest(types[i], strategy, resultType, RequestType.Other, null, true, true);

                resultArray[i] = request.Services.ExpressionBuilder.GetActivationExpression(scope, arg1Request);
            }

            return resultArray;
        }
        #endregion

        #region Create Expression to call delegate
        /// <summary>
        /// Create an expression to call delegate and apply null check and disposal logic
        /// </summary>
        /// <param name="delegateInstance"></param>
        /// <param name="allowDisposableTracking"></param>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static IActivationExpressionResult CreateExpressionForDelegate(Delegate delegateInstance, bool allowDisposableTracking, IInjectionScope scope,
            IActivationExpressionRequest request)
        {
            var methodInfo = delegateInstance.GetMethodInfo();

            var resultsExpressions = CreateExpressionsForTypes(request.RequestingStrategy, scope, request, methodInfo.ReturnType,
                methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());

            Expression expression = methodInfo.IsStatic
                ? Expression.Call(methodInfo, resultsExpressions.Select(e => e.Expression))
                : Expression.Call(Expression.Constant(delegateInstance.Target),
                    methodInfo, resultsExpressions.Select(e => e.Expression));

            expression = ApplyNullCheckAndAddDisposal(scope, request, expression, allowDisposableTracking);

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            foreach (var expressionResult in resultsExpressions)
            {
                result.AddExpressionResult(expressionResult);
            }

            return result;
        }
        #endregion

        #region Apply null check
        /// <summary>
        /// Applies null check and disposal scope tracking logic to an expression
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="expression"></param>
        /// <param name="allowDisposableTracking"></param>
        /// <returns></returns>
        public static Expression ApplyNullCheckAndAddDisposal(IInjectionScope scope, IActivationExpressionRequest request, Expression expression, bool allowDisposableTracking)
        {
            if (expression.Type != request.ActivationType &&
               !ReflectionService.CheckTypeIsBasedOnAnotherType(expression.Type, request.ActivationType))
            {
                expression = Expression.Convert(expression, request.ActivationType);
            }

            if (!allowDisposableTracking)
            {
                if (!scope.ScopeConfiguration.Behaviors.AllowInstanceAndFactoryToReturnNull)
                {
                    var closedMethod = CheckForNullMethodInfo.MakeGenericMethod(request.ActivationType);

                    return Expression.Call(closedMethod,
                                           Expression.Constant(request.GetStaticInjectionContext()),
                                           expression);
                }
            }
            else
            {
                if (scope.ScopeConfiguration.Behaviors.AllowInstanceAndFactoryToReturnNull)
                {
                    var closedMethod = AddToDisposalScopeMethodInfo.MakeGenericMethod(request.ActivationType);

                    return Expression.Call(closedMethod, request.DisposalScopeExpression, expression);
                }
                else
                {
                    var closedMethod =
                        CheckForNullAndAddToDisposalScopeMethodInfo.MakeGenericMethod(request.ActivationType);

                    return Expression.Call(closedMethod,
                                           request.DisposalScopeExpression,
                                           Expression.Constant(request.GetStaticInjectionContext()), expression);
                }
            }

            return expression;
        }

        #region Check For Null
        /// <summary>
        /// Check value for null
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="context">static context</param>
        /// <param name="value">value to check</param>
        /// <returns>non null value</returns>
        public static T CheckForNull<T>(StaticInjectionContext context, T value)
        {
            if (value == null)
            {
                throw new NullValueProvidedException(context);
            }

            return value;
        }

        private static MethodInfo _checkForNullMethodInfo;

        /// <summary>
        /// Method info for CheckForNull
        /// </summary>
        public static MethodInfo CheckForNullMethodInfo
        {
            get
            {
                return _checkForNullMethodInfo ??
                       (_checkForNullMethodInfo =
                           typeof(ExpressionUtilities).GetRuntimeMethods().First(m => m.Name == "CheckForNull"));
            }
        }

        #endregion

        #region AddToDisposalScope

        /// <summary>
        /// Add instance to disposal scope and return it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="disposalScope"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T AddToDisposalScope<T>(IDisposalScope disposalScope, T value)
        {
            var disposable = value as IDisposable;

            if (disposable != null)
            {
                disposalScope.AddDisposable(disposable);
            }

            return value;
        }

        private static MethodInfo _addToDisposalScopeMethodInfo;

        /// <summary>
        /// Method info for AddToDisposalScope
        /// </summary>
        public static MethodInfo AddToDisposalScopeMethodInfo
        {
            get
            {
                return _addToDisposalScopeMethodInfo ??
                       (_addToDisposalScopeMethodInfo =
                           typeof(ExpressionUtilities).GetRuntimeMethods().First(m => m.Name == "AddToDisposalScope"));
            }
        }
        #endregion

        #region CheckForNullAndAddToDisposalScope

        /// <summary>
        /// Check for null and then add to disposal scope
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="disposalScope"></param>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T CheckForNullAndAddToDisposalScope<T>(IDisposalScope disposalScope,
            StaticInjectionContext context, T value)
        {
            if (value == null)
            {
                throw new NullValueProvidedException(context);
            }

            var disposable = value as IDisposable;

            if (disposable != null)
            {
                disposalScope.AddDisposable(disposable);
            }

            return value;
        }

        private static MethodInfo _checkForNullAndAddToDisposalScopeMethodInfo;

        /// <summary>
        /// Method info for CheckForNullAndAddToDisposalScope
        /// </summary>
        public static MethodInfo CheckForNullAndAddToDisposalScopeMethodInfo
        {
            get
            {
                return _checkForNullAndAddToDisposalScopeMethodInfo ??
                       (_checkForNullAndAddToDisposalScopeMethodInfo =
                           typeof(ExpressionUtilities).GetRuntimeMethods().First(m => m.Name == "CheckForNullAndAddToDisposalScope"));
            }
        }
        #endregion
        #endregion
    }
}
