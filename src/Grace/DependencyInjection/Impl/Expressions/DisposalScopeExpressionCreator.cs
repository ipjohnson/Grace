using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Interface for creating linq expression to add instance to disposal scope
    /// </summary>
    public interface IDisposalScopeExpressionCreator
    {
        /// <summary>
        /// Create expression to add instance to disposal scope
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <param name="result">result for instantiation</param>
        /// <returns></returns>
        IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result);
    }

    /// <summary>
    /// Creates linq expression that add instance to disposal scope
    /// </summary>
    public class DisposalScopeExpressionCreator : IDisposalScopeExpressionCreator
    {
        private MethodInfo _addMethod;

        /// <summary>
        /// Create expression to add instance to disposal scope
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <param name="result">result for instantiation</param>
        /// <returns></returns>
        public IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result)
        {
            var closedGeneric = AddMethod.MakeGenericMethod(activationConfiguration.ActivationType);

            var closedActionType = typeof(Action<>).MakeGenericType(activationConfiguration.ActivationType);

            object disposalDelegate = null;

            if (closedActionType == activationConfiguration.DisposalDelegate?.GetType())
            {
                disposalDelegate = activationConfiguration.DisposalDelegate;
            }

            var disposalCall =
                Expression.Call(request.DisposalScopeExpression, closedGeneric, result.Expression, Expression.Convert(Expression.Constant(disposalDelegate), closedActionType));

            var disposalResult = request.Services.Compiler.CreateNewResult(request, disposalCall);

            disposalResult.AddExpressionResult(result);

            return disposalResult;
        }

        /// <summary>
        /// Method info for add method on IDisposalScope
        /// </summary>
        protected MethodInfo AddMethod
        {
            get
            {
                return _addMethod ??
                    (_addMethod = typeof(IDisposalScope).GetTypeInfo().DeclaredMethods.First(m => m.Name == "AddDisposable"));
            }
        }
    }
}
