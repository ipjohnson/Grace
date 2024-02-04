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
        /// <param name="rootKey">key for Root request</param>
        IActivationExpressionResult GetEnumerableExpression(
            IInjectionScope scope, 
            IActivationExpressionRequest request, 
            IArrayExpressionCreator arrayExpressionCreator,
            object rootKey);
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
        /// <param name="rootKey">key for Root request</param>
        public IActivationExpressionResult GetEnumerableExpression(
            IInjectionScope scope, 
            IActivationExpressionRequest request,
            IArrayExpressionCreator arrayExpressionCreator,
            object rootKey)
        {
            return scope.ScopeConfiguration.Behaviors.CustomEnumerableCreator is {} enumerableCreator
                ? CreateEnumerableExpressionUsingCustomCreator(scope, request, arrayExpressionCreator, enumerableCreator, rootKey)
                : CreateEnumerableExpressionUsingArrayExpression(scope, request, arrayExpressionCreator, rootKey);
        }

        /// <summary>
        /// Create enumerable expression that is an array
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="arrayExpressionCreator"></param>
        /// <param name="rootKey"></param>
        protected virtual IActivationExpressionResult CreateEnumerableExpressionUsingArrayExpression(
            IInjectionScope scope,
            IActivationExpressionRequest request, 
            IArrayExpressionCreator arrayExpressionCreator,
            object rootKey)
        {
            var enumerableType = request.ActivationType.GenericTypeArguments[0];

            var arrayType = enumerableType.MakeArrayType();

            var newRequest = request.NewRequest(arrayType, request.RequestingStrategy,
                request.RequestingStrategy?.ActivationType, request.RequestType, request.Info, true, true);

            newRequest.SetFilter(request.Filter);
            newRequest.SetEnumerableComparer(request.EnumerableComparer);
            newRequest.SetLocateKey(request.LocateKey);

            return arrayExpressionCreator.GetArrayExpression(scope, newRequest, rootKey);
        }

        /// <summary>
        /// Create enumerable expression using a custom creator
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="request">expression request</param>
        /// <param name="arrayExpressionCreator">array creator</param>
        /// <param name="enumerableCreator">custom enumerable creator</param>
        /// <param name="rootKey">key for Root requests</param>
        protected virtual IActivationExpressionResult CreateEnumerableExpressionUsingCustomCreator(
            IInjectionScope scope,
            IActivationExpressionRequest request, 
            IArrayExpressionCreator arrayExpressionCreator,
            IEnumerableCreator enumerableCreator,
            object rootKey)
        {
            var arrayExpression = CreateEnumerableExpressionUsingArrayExpression(scope, request, arrayExpressionCreator, rootKey);

            var enumerableType = request.ActivationType.GenericTypeArguments[0];

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
        protected MethodInfo CreateEnumerableMethod => _createEnumerableMethod ??= typeof(IEnumerableCreator).GetRuntimeMethods().First();

        #endregion
    }

}
