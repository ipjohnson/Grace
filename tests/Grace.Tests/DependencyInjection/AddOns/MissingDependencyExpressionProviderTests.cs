using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using Grace.Tests.DependencyInjection.Lifestyle;
using Xunit;

namespace Grace.Tests.DependencyInjection.AddOns
{
    public class MissingDependencyExpressionProviderTests
    {
        [Fact]
        public void MissingDependenyExpressionProviderTest()
        {
            var container = new DependencyInjectionContainer
            {
                c =>
                {
                    c.AddMissingDependencyExpressionProvider(new ChildContainerExpressionProvider());
                    c.Export<DependentService<IBasicService>>().As<IDependentService<IBasicService>>();
                }
            };

            using (var childContainer = container.CreateChildScope(c => c.Export<BasicService>().As<IBasicService>()))
            {
                using (var secondChild = childContainer.CreateChildScope())
                {
                    var instance = secondChild.Locate<IDependentService<IBasicService>>();

                    Assert.NotNull(instance);

                }
            }
        }

        [Fact]
        public void MissingDependenyExpressionProviderThrowsExceptionTest()
        {
            var container = new DependencyInjectionContainer
            {
                c =>
                {
                    c.AddMissingDependencyExpressionProvider(new ChildContainerExpressionProvider());
                    c.Export<DependentService<IBasicService>>().As<IDependentService<IBasicService>>();
                }
            };

            var childContainer = container.CreateChildScope();

            Assert.Throws<LocateException>(() => childContainer.Locate<IDependentService<IBasicService>>());
        }
        
        public class ChildContainerExpressionProvider : IMissingDependencyExpressionProvider
        {
            /// <summary>
            /// Provide linq expression for the missing dependency
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="request"></param>
            /// <returns>return expression or null if it's not possible</returns>
            public IActivationExpressionResult ProvideExpression(IInjectionScope scope, IActivationExpressionRequest request)
            {
                var valueProvider =
                    scope.ScopeConfiguration.Implementation.Locate<IInjectionContextValueProvider>();

                var key = request.LocateKey;

                if (key is string)
                {
                    key = ((string)key).ToLowerInvariant();
                }

                var locateFromChildMethod = GetType().GetTypeInfo().GetDeclaredMethod("LocateFromChildContainer");

                Expression expression = Expression.Call(Expression.Constant(this),
                    locateFromChildMethod,
                    Expression.Constant(valueProvider),
                    request.ScopeParameter,
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
}
