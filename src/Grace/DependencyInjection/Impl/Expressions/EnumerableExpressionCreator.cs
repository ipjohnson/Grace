using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// interface for creating enumerable expressions
    /// </summary>
    public interface IEnumerableExpressionCreator
    {
        /// <summary>
        /// Get expression for creating enumerable
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <param name="arrayExpressionCreator">array expression creator</param>
        /// <returns></returns>
        IActivationExpressionResult GetEnumerableExpression(IInjectionScope scope, IActivationExpressionRequest request, IArrayExpressionCreator arrayExpressionCreator);
    }

    /// <summary>
    /// class for creating enumerable expressions
    /// </summary>
    public class EnumerableExpressionCreator : IEnumerableExpressionCreator
    {
        /// <summary>
        /// Get expression for creating enumerable
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <param name="arrayExpressionCreator">array expression creator</param>
        /// <returns></returns>
        public IActivationExpressionResult GetEnumerableExpression(IInjectionScope scope, IActivationExpressionRequest request,
            IArrayExpressionCreator arrayExpressionCreator)
        {
            var enumerableCreator = scope.ScopeConfiguration.Behaviors.CustomEnumerableCreator;

            return enumerableCreator != null
                ? CreateEnumerableExpressionUsingCustomCreator(scope, request, arrayExpressionCreator, enumerableCreator)
                : CreateEnumerableExpressionUsingArrayExpression(scope, request, arrayExpressionCreator);
        }

        /// <summary>
        /// Create enumerable expression that is an array
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="arrayExpressionCreator"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult CreateEnumerableExpressionUsingArrayExpression(IInjectionScope scope,
            IActivationExpressionRequest request, IArrayExpressionCreator arrayExpressionCreator)
        {
            var enumerableType = request.ActivationType.GenericTypeArguments[0];

            var arrayType = enumerableType.MakeArrayType();

            var newRequest = request.NewRequest(arrayType, request.RequestingStrategy,
                request.RequestingStrategy?.ActivationType, request.RequestType, request.Info, true, true);

            newRequest.SetFilter(request.Filter);
            newRequest.SetEnumerableComparer(request.EnumerableComparer);
            newRequest.SetLocateKey(request.LocateKey);

            var arrayExpression = arrayExpressionCreator.GetArrayExpression(scope, newRequest);

            return arrayExpression;
        }

        /// <summary>
        /// Create enumerable expression using a custom creator
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="request">expression request</param>
        /// <param name="arrayExpressionCreator">array creator</param>
        /// <param name="enumerableCreator">custom enumerable creator</param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult CreateEnumerableExpressionUsingCustomCreator(IInjectionScope scope,
            IActivationExpressionRequest request, IArrayExpressionCreator arrayExpressionCreator,
            IEnumerableCreator enumerableCreator)
        {
            var enumerableType = request.ActivationType.GenericTypeArguments[0];

            var arrayType = enumerableType.MakeArrayType();

            var newRequest =
                request.NewRequest(arrayType, request.RequestingStrategy,
                    request.RequestingStrategy?.ActivationType, request.RequestType, request.Info, true, true);

            newRequest.SetFilter(request.Filter);
            newRequest.SetEnumerableComparer(request.EnumerableComparer);

            var arrayExpression = arrayExpressionCreator.GetArrayExpression(scope, newRequest);

            request.RequireExportScope();

            var enumerableExpression = 
                Expression.Call(Expression.Constant(enumerableCreator),
                    CreateEnumerableMethod.MakeGenericMethod(enumerableType),
                    request.ScopeParameter,
                    arrayExpression.Expression);

            var returnResult = request.Services.Compiler.CreateNewResult(request, enumerableExpression);

            returnResult.AddExpressionResult(returnResult);

            return returnResult;
        }

        #region CreateEnumerableMethod
        private MethodInfo _createEnumerableMethod;

        /// <summary>
        /// Create enumerable method
        /// </summary>
        protected MethodInfo CreateEnumerableMethod => _createEnumerableMethod ??
                                                       (_createEnumerableMethod = typeof(IEnumerableCreator).GetRuntimeMethods().First());

        #endregion
    }

}
