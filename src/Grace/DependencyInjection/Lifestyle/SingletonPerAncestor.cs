using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Singleton per ancestor
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplayValue) + ",nq}")]
    public class SingletonPerAncestor : ICompiledLifestyle
    {
        private readonly Type _ancestorType;
        private readonly bool _guaranteeOnlyOne;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="ancestorType"></param>
        /// <param name="guaranteeOnlyOne"></param>
        public SingletonPerAncestor(Type ancestorType, bool guaranteeOnlyOne)
        {
            _ancestorType = ancestorType;
            _guaranteeOnlyOne = guaranteeOnlyOne;
        }
        
        /// <summary>
        /// Generalization for lifestyle
        /// </summary>
        public LifestyleType LifestyleType { get; } = LifestyleType.Transient;

        /// <summary>
        /// Clone the lifestyle
        /// </summary>
        /// <returns></returns>
        public ICompiledLifestyle Clone()
        {
            return new SingletonPerAncestor(_ancestorType, _guaranteeOnlyOne);
        }

        /// <summary>
        /// Provide an expression that uses the lifestyle
        /// </summary>
        /// <param name="scope">scope for the strategy</param>
        /// <param name="request">activation request</param>
        /// <param name="activationExpression">expression to create strategy type</param>
        /// <returns></returns>
        public IActivationExpressionResult ProvideLifestyleExpression(IInjectionScope scope, IActivationExpressionRequest request, Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
        {
            var context = request.GetStaticInjectionContext();

            var ancestorId = GetAncestorRequestId(context);
            var newDelegate = request.Services.Compiler.CompileDelegate(scope, activationExpression(request));

            MethodInfo closedMethod;

            if (_guaranteeOnlyOne)
            {
                var openMethod = typeof(SingletonPerAncestor).GetRuntimeMethod(nameof(GetValueGuaranteeOnce),
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
                var openMethod = typeof(SingletonPerAncestor).GetRuntimeMethod(nameof(GetValue),
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

            var expression = Expression.Call(closedMethod, request.ScopeParameter,
                request.DisposalScopeExpression, request.InjectionContextParameter,
                Expression.Constant(newDelegate), Expression.Constant(ancestorId));

            request.RequireInjectionContext();
            request.RequireExportScope();
            
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
                throw new LocateException(context, $"Could not find ancestor type {_ancestorType.Name}");
            }

            return injectionInfoTarget.UniqueId;
        }

        private string DebuggerDisplayValue => $"Singleton Per Ancestor ({_ancestorType.Name})";

        /// <summary>
        /// Get value without locking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="context"></param>
        /// <param name="activationDelegate"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Get a value using lock to guarantee only one is created
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="context"></param>
        /// <param name="activationDelegate"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public static T GetValueGuaranteeOnce<T>(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context, ActivationStrategyDelegate activationDelegate, string uniqueId)
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
