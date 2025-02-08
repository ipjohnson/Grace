using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Expression utilities
    /// </summary>
    public static class ExpressionUtilities
    {
        #region CreateExpressionsForParameters
        /// <summary>
        /// Create an array of expressions based off an array of parameters
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="resultType"></param>
        /// <param name="parameters"></param>
        /// <param name="isActivationStrategyDelegate"></param>
        public static IActivationExpressionResult[] CreateExpressionsForParameters(
            IActivationStrategy strategy, 
            IInjectionScope scope,
            IActivationExpressionRequest request, 
            Type resultType, 
            IEnumerable<ParameterInfo> parameters, 
            bool isActivationStrategyDelegate)
        {
            return parameters
                .Select((p, i) => 
                {
                    var argRequest = request.NewRequest(p.ParameterType, strategy, resultType, RequestType.MethodParameter, null, true, true);
                    
                    if (isActivationStrategyDelegate)
                    {
                        if (i == 3)
                        {
                            argRequest.SetLocateKey(ImportKey.Key);
                        }
                    }
                    else if (ImportAttributeInfo.For(p, p.ParameterType, p.Name) is { ImportKey: { } key})
                    {
                        argRequest.SetLocateKey(key);
                    }

                    return argRequest.Services.ExpressionBuilder.GetActivationExpression(scope, argRequest);
                })
                .ToArray();
        }
        #endregion

        #region Create Expression to call delegate

        private const string _closureName = "System.Runtime.CompilerServices.Closure";

        /// <summary>
        /// Create an expression to call delegate and apply null check and disposal logic
        /// </summary>
        /// <param name="delegateInstance"></param>
        /// <param name="allowDisposableTracking"></param>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="requestingStrategy"></param>
        public static IActivationExpressionResult CreateExpressionForDelegate(Delegate delegateInstance, bool allowDisposableTracking, IInjectionScope scope,
            IActivationExpressionRequest request, IActivationStrategy requestingStrategy)
        {
            var methodInfo = delegateInstance.GetMethodInfo();
            var parameters = methodInfo.GetParameters();

            Expression expression = null;
            IActivationExpressionResult[] resultsExpressions;

            // Handle closure based delegates differently
            if (delegateInstance.Target != null && delegateInstance.Target.GetType().FullName == _closureName)
            {
                resultsExpressions = CreateExpressionsForParameters(
                    requestingStrategy, 
                    scope, 
                    request, 
                    methodInfo.ReturnType,
                    parameters.Where(p => p.Position != 0 || p.ParameterType.FullName != "System.Runtime.CompilerServices.Closure"),
                    IsActivationStrategyDelegate(parameters, true));
                
                expression = Expression.Invoke(Expression.Constant(delegateInstance),
                    resultsExpressions.Select(e => e.Expression));

            }
            else
            {
                resultsExpressions = CreateExpressionsForParameters(
                    requestingStrategy, 
                    scope, 
                    request, 
                    methodInfo.ReturnType,
                    parameters,
                    IsActivationStrategyDelegate(parameters, false));

                expression = methodInfo.IsStatic
                    ? Expression.Call(methodInfo, resultsExpressions.Select(e => e.Expression))
                    : Expression.Call(Expression.Constant(delegateInstance.Target),
                        methodInfo, resultsExpressions.Select(e => e.Expression));
            }

            var allowNull = (requestingStrategy as IInstanceActivationStrategy)?.AllowNullReturn ?? false;

            expression = ApplyNullCheckAndAddDisposal(scope, request, expression, allowDisposableTracking, allowNull);

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            foreach (var expressionResult in resultsExpressions)
            {
                result.AddExpressionResult(expressionResult);
            }

            return result;

            
            // This is somewhat hacky.
            // Sometimes, e.g. when called by DynamicConstructorExpressionCreator, delegateInstance is ActivateStrategyDelegate.
            // The problem in that case is that the 4th parameter, "object key", is not satisfiable without passing a LocateKey.
            bool IsActivationStrategyDelegate(ParameterInfo[] ps, bool maybeClosure)
            {                
                if (ps.Length < 4)
                {
                    return false;
                }

                int i = maybeClosure && ps[0].ParameterType.FullName == "System.Runtime.CompilerServices.Closure" ? 1 : 0;

                // Note that there's no name information available on the 4th parameter for additional verification :(
                return ps.Length == i + 4
                    && ps[i + 0].ParameterType == typeof(IExportLocatorScope)
                    && ps[i + 1].ParameterType == typeof(IDisposalScope)
                    && ps[i + 2].ParameterType == typeof(IInjectionContext)
                    && ps[i + 3].ParameterType == typeof(object);
            }
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
        /// <param name="allowNull"></param>
        public static Expression ApplyNullCheckAndAddDisposal(IInjectionScope scope,
            IActivationExpressionRequest request, Expression expression, bool allowDisposableTracking, bool allowNull)
        {
            if (expression.Type != request.ActivationType &&
               !ReflectionService.CheckTypeIsBasedOnAnotherType(expression.Type, request.ActivationType))
            {
                expression = Expression.Convert(expression, request.ActivationType);
            }

            if (!allowDisposableTracking)
            {
                if (request.DefaultValue != null)
                {
                    var method = typeof(ExpressionUtilities).GetRuntimeMethods()
                           .FirstOrDefault(m => m.Name == nameof(ValueOrDefault));

                    var closedMethod = method.MakeGenericMethod(request.ActivationType);

                    return Expression.Call(closedMethod, expression, Expression.Constant(request.DefaultValue.DefaultValue, request.ActivationType));
                }

                if (!allowNull &&
                    !scope.ScopeConfiguration.Behaviors.AllowInstanceAndFactoryToReturnNull &&
                    request.IsRequired)
                {
                    var closedMethod = CheckForNullMethodInfo.MakeGenericMethod(request.ActivationType);

                    return Expression.Call(closedMethod,
                                           Expression.Constant(request.GetStaticInjectionContext()),
                                           expression);
                }

                return expression;

            }

            if (request.DefaultValue != null)
            {
                request.RequireDisposalScope();

                var method = typeof(ExpressionUtilities).GetRuntimeMethods()
                    .FirstOrDefault(m => m.Name == nameof(AddToDisposableScopeOrDefault));

                var closedMethod = method.MakeGenericMethod(request.ActivationType);

                return Expression.Call(closedMethod, request.DisposalScopeExpression, expression, Expression.Constant(request.DefaultValue.DefaultValue, request.ActivationType));
            }

            if (allowNull ||
                scope.ScopeConfiguration.Behaviors.AllowInstanceAndFactoryToReturnNull ||
                !request.IsRequired)
            {
                request.RequireDisposalScope();

                var closedMethod = AddToDisposalScopeMethodInfo.MakeGenericMethod(request.ActivationType);

                return Expression.Call(closedMethod, request.DisposalScopeExpression, expression);
            }
            else
            {
                request.RequireDisposalScope();

                var closedMethod = CheckForNullAndAddToDisposalScopeMethodInfo.MakeGenericMethod(request.ActivationType);

                return Expression.Call(closedMethod,
                                       request.DisposalScopeExpression,
                                       Expression.Constant(request.GetStaticInjectionContext()), expression);
            }
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
                return _checkForNullMethodInfo = _checkForNullMethodInfo ?? typeof(ExpressionUtilities).GetRuntimeMethods().First(m => m.Name == nameof(CheckForNull));
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
        public static T AddToDisposalScope<T>(IDisposalScope disposalScope, T value)
        {

#if NET6_0_OR_GREATER
            if (value is IAsyncDisposable asyncDisposable)
            {
                disposalScope.AddAsyncDisposable(asyncDisposable);
            }
#endif

            if (value is IDisposable disposable)
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
                return _addToDisposalScopeMethodInfo = _addToDisposalScopeMethodInfo ?? typeof(ExpressionUtilities).GetRuntimeMethods().First(m => m.Name == nameof(AddToDisposalScope));
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
        public static T CheckForNullAndAddToDisposalScope<T>(IDisposalScope disposalScope,
            StaticInjectionContext context, T value)
        {
            if (value == null)
            {
                throw new NullValueProvidedException(context);
            }

            return AddToDisposalScope(disposalScope, value);
        }

        private static MethodInfo _checkForNullAndAddToDisposalScopeMethodInfo;

        /// <summary>
        /// Method info for CheckForNullAndAddToDisposalScope
        /// </summary>
        public static MethodInfo CheckForNullAndAddToDisposalScopeMethodInfo
        {
            get
            {
                return _checkForNullAndAddToDisposalScopeMethodInfo = _checkForNullAndAddToDisposalScopeMethodInfo ?? typeof(ExpressionUtilities).GetRuntimeMethods().First(m => m.Name == nameof(CheckForNullAndAddToDisposalScope));
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tValue"></param>
        /// <param name="defaultValue"></param>
        public static T ValueOrDefault<T>(T tValue, T defaultValue)
        {
            return tValue != null ? tValue : defaultValue;
        }

        #region AddToDisposableScopeOrDefault

        /// <summary>
        /// Add to disposal scope or use default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="disposalScope"></param>
        /// <param name="tValue"></param>
        /// <param name="defaultValue"></param>
        public static T AddToDisposableScopeOrDefault<T>(IDisposalScope disposalScope, T tValue, T defaultValue)
        {
            if (tValue != null)
            {
                return AddToDisposalScope(disposalScope, tValue);
            }

            return defaultValue;
        }

        #endregion

        #endregion
    }
}
