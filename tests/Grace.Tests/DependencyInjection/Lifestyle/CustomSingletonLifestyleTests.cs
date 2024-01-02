using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class CustomSingletonLifestyleTests
    {
        /// <summary>
        /// Custom singleton implementation that passes context through during construction
        /// </summary>
        public class CustomSingletonLifestyle : ICompiledLifestyle
        {
            private volatile object _singleton;
            private readonly object _lockObject = new object();
            private ActivationStrategyDelegate _activationDelegate;

            /// <summary>
            /// Constant expression
            /// </summary>
            protected Expression ConstantExpression;

            public LifestyleType LifestyleType => LifestyleType.Singleton;

            public ICompiledLifestyle Clone()
            {
                return new CustomSingletonLifestyle();
            }

            public IActivationExpressionResult ProvideLifestyleExpression(IInjectionScope scope, IActivationExpressionRequest request, Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
            {
                if (ConstantExpression != null)
                {
                    return request.Services.Compiler.CreateNewResult(request, ConstantExpression);
                }

                // Create new request as we shouldn't carry over anything from the previous request
                var newRequest = request.NewRootedRequest(request.ActivationType, scope, true);

                _activationDelegate = request.Services.Compiler.CompileDelegate(scope, activationExpression(newRequest));

                var singletonMethod = GetType().GetTypeInfo().GetDeclaredMethod(nameof(SingletonActivation));

                ConstantExpression = Expression.Call(
                    Expression.Constant(this), 
                    singletonMethod,
                    request.Constants.ScopeParameter, 
                    request.Constants.RootDisposalScope,
                    request.Constants.InjectionContextParameter,
                    request.GetKeyExpression());

                return request.Services.Compiler.CreateNewResult(request, ConstantExpression);
            }

            private object SingletonActivation(
                IExportLocatorScope scope, 
                IDisposalScope disposalScope,
                IInjectionContext context,
                object key)
            {
                if (_singleton != null)
                {
                    return _singleton;
                }

                lock (_lockObject)
                {
                    if (_singleton == null)
                    {
                        _singleton = _activationDelegate(scope, disposalScope, context, key);
                    }
                }

                return _singleton;
            }
        }

        [Fact]
        public void CustomSingletonTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DependentService<IBasicService>>().As<IDependentService<IBasicService>>().Lifestyle.Custom(new CustomSingletonLifestyle()));

            var instance = container.Locate<IDependentService<IBasicService>>(new { basicService = new BasicService { Count = 5 } });

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.Equal(5, instance.Value.Count);

            var instance2 = container.Locate<IDependentService<IBasicService>>(new { basicService = new BasicService { Count = 15 } });
            
            Assert.Same(instance, instance2);
        }
    }
}
