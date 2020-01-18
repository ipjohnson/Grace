using System;
using System.Linq.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Strategy that represents a constant value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConstantInstanceExportStrategy<T> : BaseInstanceExportStrategy
    {
        private readonly T _constant;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="constant"></param>
        /// <param name="injectionScope"></param>
        public ConstantInstanceExportStrategy(T constant, IInjectionScope injectionScope) : base(GetBaseType(constant), injectionScope)
        {
            _constant = constant;
        }

        /// <summary>
        /// Create expression that is implemented in child class
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        protected override IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            if (lifestyle == null)
            {
                return CreateExpression(request);
            }
            return lifestyle.ProvideLifestyleExpression(scope, request, CreateExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult CreateExpression(IActivationExpressionRequest request)
        {
            var expressionStatement = Expression.Constant(_constant);

            return request.Services.Compiler.CreateNewResult(request, expressionStatement);
        }

        private static Type GetBaseType(T constant)
        {
            return typeof(T) == typeof(object) && constant != null ? constant.GetType() : typeof(T);
        }
    }
}
