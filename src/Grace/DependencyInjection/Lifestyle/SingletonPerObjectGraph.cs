using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Lifestyle
{
    public class SingletonPerObjectGraph : ICompiledLifestyle
    {
        private readonly bool _guaranteeOnlyOne;
        private readonly string _uniqueId = Guid.NewGuid().ToString();

        public SingletonPerObjectGraph(bool guaranteeOnlyOne)
        {
            _guaranteeOnlyOne = guaranteeOnlyOne;
        }

        public bool RootRequest { get; } = false;

        public ICompiledLifestyle Clone()
        {
            return new SingletonPerObjectGraph(_guaranteeOnlyOne);
        }

        public IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope, IActivationExpressionRequest request,
            IActivationExpressionResult activationExpression)
        {
            var newDelegate = request.Services.Compiler.CompileDelegate(scope, activationExpression);

            MethodInfo closedMethod;

            if (_guaranteeOnlyOne)
            {
                var openMethod = typeof(SingletonPerAncestor).GetRuntimeMethod("GetValueGuaranteeOnce",
                    new[]
                    {
                        typeof(IExportLocatorScope),
                        typeof(IDisposalScope),
                        typeof(IInjectionContext),
                        typeof(ActivationStrategyDelegate),
                        typeof(string)
                    });

                closedMethod = openMethod.MakeGenericMethod(request.ActivationType);
            }
            else
            {
                var openMethod = typeof(SingletonPerAncestor).GetRuntimeMethod("GetValue",
                    new[]
                    {
                        typeof(IExportLocatorScope),
                        typeof(IDisposalScope),
                        typeof(IInjectionContext),
                        typeof(ActivationStrategyDelegate),
                        typeof(string)
                    });

                closedMethod = openMethod.MakeGenericMethod(request.ActivationType);
            }

            var expression = Expression.Call(closedMethod, request.Constants.ScopeParameter,
                request.DisposalScopeExpression, request.Constants.InjectionContextParameter,
                Expression.Constant(newDelegate), Expression.Constant(_uniqueId));

            request.RequireInjectionContext();

            return request.Services.Compiler.CreateNewResult(request, expression);
        }

        public static T GetValue<T>(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context, ActivationStrategyDelegate activationDelegate, string uniqueId)
        {
            var value = context.SharedData.GetExtraData(uniqueId);

            if (value != null)
            {
                return (T)value;
            }

            value = activationDelegate(scope, disposalScope, context);

            context.SharedData.SetExtraData(uniqueId, value);

            return (T)value;
        }

        public T GetValueGuaranteeOnce<T>(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context, ActivationStrategyDelegate activationDelegate, string uniqueId)
        {
            var value = context.SharedData.GetExtraData(uniqueId);

            if (value == null)
            {
                lock (context.SharedData.GetLockObject("SingletonPerObjectGraph|" + uniqueId))
                {
                    value = context.SharedData.GetExtraData(uniqueId);

                    if (value == null)
                    {
                        value = activationDelegate(scope, disposalScope, context);

                        context.SharedData.SetExtraData(uniqueId, value);
                    }
                }
            }

            return (T)value;
        }
    }
}
