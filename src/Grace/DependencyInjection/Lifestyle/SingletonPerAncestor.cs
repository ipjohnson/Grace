using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Lifestyle
{
    public class SingletonPerAncestor : ICompiledLifestyle
    {
        private readonly Type _ancestorType;
        private readonly bool _guaranteeOnlyOne;

        public SingletonPerAncestor(Type ancestorType, bool guaranteeOnlyOne)
        {
            _ancestorType = ancestorType;
            _guaranteeOnlyOne = guaranteeOnlyOne;
        }

        public bool RootRequest { get; } = true;

        public ICompiledLifestyle Clone()
        {
            return new SingletonPerAncestor(_ancestorType, _guaranteeOnlyOne);
        }

        public IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope,
                                                                     IActivationExpressionRequest request,
                                                                     IActivationExpressionResult activationExpression)
        {
            var context = request.GetStaticInjectionContext();

            var ancestorId = GetAncestorRequestId(context);
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
                Expression.Constant(newDelegate), Expression.Constant(ancestorId));

            return request.Services.Compiler.CreateNewResult(request, expression);
        }

        private string GetAncestorRequestId(StaticInjectionContext context)
        {
            var typeInfo = _ancestorType.GetTypeInfo();

            var injectionInfoTarget = context.InjectionStack.FirstOrDefault(target =>
            {
                var injectionInfo = target.InjectionType?.GetTypeInfo();

                if (injectionInfo != null && typeInfo.IsAssignableFrom(injectionInfo))
                {
                    return true;
                }

                return false;
            });

            if (injectionInfoTarget == null)
            {
                // todo fix ancestor exception
                throw new Exception("Could not find ancestor type");
            }

            return injectionInfoTarget.UniqueId;
        }

        public static T GetValue<T>(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context, ActivationStrategyDelegate activationDelegate,
            string uniqueId)
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
                lock (context.SharedData.GetLockObject("SingletonPerAncestor|" + uniqueId))
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
