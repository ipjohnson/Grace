using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Utilities;

namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Singleton per object graph
    /// </summary>
    [DebuggerDisplay("Singleton Per Object Graph")]
    public class SingletonPerObjectGraph : ICompiledLifestyle
    {
        private readonly bool _guaranteeOnlyOne;
        private readonly string _uniqueId = UniqueStringId.Generate();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="guaranteeOnlyOne"></param>
        public SingletonPerObjectGraph(bool guaranteeOnlyOne)
        {
            _guaranteeOnlyOne = guaranteeOnlyOne;
        }
        
        /// <summary>
        /// Generalization for lifestyle
        /// </summary>
        public LifestyleType LifestyleType { get; } = LifestyleType.Transient;

        /// <summary>
        /// Clone the lifestyle
        /// </summary>
        public ICompiledLifestyle Clone()
        {
            return new SingletonPerObjectGraph(_guaranteeOnlyOne);
        }

        /// <summary>
        /// Provide an expression that uses the lifestyle
        /// </summary>
        /// <param name="scope">scope for the strategy</param>
        /// <param name="request">activation request</param>
        /// <param name="activationExpression">expression to create strategy type</param>
        public IActivationExpressionResult ProvideLifestyleExpression(IInjectionScope scope, IActivationExpressionRequest request, Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
        {
            var newDelegate = request.Services.Compiler.CompileDelegate(scope, activationExpression(request));

            var openMethod = typeof(SingletonPerObjectGraph).GetRuntimeMethod(
                _guaranteeOnlyOne ? nameof(GetValueGuaranteeOnce) : nameof(GetValue),
                new[]
                {
                    typeof(IExportLocatorScope),
                    typeof(IDisposalScope),
                    typeof(IInjectionContext),
                    typeof(object),
                    typeof(ActivationStrategyDelegate),
                    typeof(string)
                });

            var closedMethod = openMethod.MakeGenericMethod(request.ActivationType);

            var expression = Expression.Call(
                closedMethod, 
                request.ScopeParameter,
                request.DisposalScopeExpression, 
                request.InjectionContextParameter,
                request.Constants.KeyParameter,
                Expression.Constant(newDelegate), 
                Expression.Constant(_uniqueId));

            request.RequireInjectionContext();
            request.RequireExportScope();

            return request.Services.Compiler.CreateNewResult(request, expression);
        }

        /// <summary>
        /// Get value for object graph
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="context"></param>
        /// <param name="activationDelegate"></param>
        /// <param name="uniqueId"></param>
        public static T GetValue<T>(
            IExportLocatorScope scope, 
            IDisposalScope disposalScope, 
            IInjectionContext context, 
            object key,
            ActivationStrategyDelegate activationDelegate, 
            string uniqueId)
        {
            return (T)(context.SharedData.GetExtraData(uniqueId) ??
                       context.SharedData.SetExtraData(uniqueId, activationDelegate(scope, disposalScope, context, key), false));
        }

        /// <summary>
        /// Get value from context guarantee only one is created using lock
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="context"></param>
        /// <param name="activationDelegate"></param>
        /// <param name="uniqueId"></param>
        public static T GetValueGuaranteeOnce<T>(
            IExportLocatorScope scope, 
            IDisposalScope disposalScope, 
            IInjectionContext context, 
            object key,
            ActivationStrategyDelegate activationDelegate, 
            string uniqueId)
        {
            var value = context.SharedData.GetExtraData(uniqueId);

            if (value == null)
            {
                lock (context.SharedData.GetLockObject("SingletonPerObjectGraph|" + uniqueId))
                {
                    value = context.SharedData.GetExtraData(uniqueId);

                    if (value == null)
                    {
                        value = activationDelegate(scope, disposalScope, context, key);

                        context.SharedData.SetExtraData(uniqueId, value);
                    }
                }
            }

            return (T)value;
        }
    }
}
