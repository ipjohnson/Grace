using System;
using System.Linq.Expressions;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Strategy for export Expression Tree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExpressionExportStrategy<T> : BaseInstanceExportStrategy
    {
        private readonly Expression<Func<T>> _expression;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="injectionScope"></param>
        public ExpressionExportStrategy(Expression<Func<T>> expression, IInjectionScope injectionScope) : base(typeof(T), injectionScope)
        {
            _expression = expression;
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
            var replacer = new LocateExpressionReplacer(request, this);

            var result = replacer.Replace(_expression);

            var funcExpression = (Expression<Func<T>>)result.Expression;

            result.Expression = funcExpression.Body;

            return result;
        }
    }
}
