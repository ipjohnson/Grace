using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Similar to Best Match but if a match isn't found for a dependency child containers will be inspected
    /// </summary>
    public class ChildContainerConstructorExpressionCreator : BestMatchConstructorExpressionCreator
    {
        /// <summary>
        /// Allow constructor selector to override method
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="configuration"></param>
        /// <param name="request"></param>
        /// <param name="constructor"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override IActivationExpressionResult OverrideExpression(IInjectionScope injectionScope,
            TypeActivationConfiguration configuration, IActivationExpressionRequest request, ConstructorInfo constructor,
            IActivationExpressionResult expression)
        {
            if (expression.UsingFallbackExpression)
            {
                return CreateChildContainerLocateExpression(injectionScope, configuration, request, constructor);
            }

            return expression;
        }

        private IActivationExpressionResult CreateChildContainerLocateExpression(IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request, ConstructorInfo constructor)
        {
            var valueProvider =
                injectionScope.ScopeConfiguration.Implementation.Locate<IInjectionContextValueProvider>();

            var key = request.LocateKey;

            if (key is string)
            {
                key = ((string)key).ToLowerInvariant();
            }

            var locateFromChildMethod = GetType().GetTypeInfo().GetDeclaredMethod("LocateFromChildContainer");

            Expression expression = Expression.Call(Expression.Constant(this),
                locateFromChildMethod,
                Expression.Constant(valueProvider),
                request.Constants.ScopeParameter,
                Expression.Constant(request.GetStaticInjectionContext()),
                Expression.Constant(request.ActivationType),
                Expression.Constant(key),
                request.Constants.InjectionContextParameter,
                Expression.Constant(request.DefaultValue?.DefaultValue),
                Expression.Constant(request.DefaultValue != null),
                Expression.Constant(request.IsRequired));

            expression = Expression.Convert(expression, request.ActivationType);

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            result.UsingFallbackExpression = true;

            return result;
        }

        protected virtual object LocateFromChildContainer(IInjectionContextValueProvider valueProvider,
            IExportLocatorScope scope,
            StaticInjectionContext staticContext,
            Type type,
            object key,
            IInjectionContext context,
            object defaultValue,
            bool useDefault,
            bool isRequired)
        {
            var value = valueProvider.GetValueFromInjectionContext(scope, type, key, context, false);

            if (value != null)
            {
                return value;
            }

            if (scope.TryLocate(type, out value, context, withKey: key))
            {
                return value;
            }

            if (useDefault)
            {
                return defaultValue;
            }

            if (isRequired)
            {
                throw new LocateException(staticContext);
            }

            return null;
        }
    }
}
