using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public interface IEnumerableExpressionCreator
    {
        IActivationExpressionResult GetEnumerableExpression(IInjectionScope scope, IActivationExpressionRequest request, IArrayExpressionCreator arrayExpressionCreator);
    }

    public class EnumerableExpressionCreator : IEnumerableExpressionCreator
    {
        public IActivationExpressionResult GetEnumerableExpression(IInjectionScope scope, IActivationExpressionRequest request,
            IArrayExpressionCreator arrayExpressionCreator)
        {
            var enumerableCreator =
                ((IExportCompilationBehaviorValues)scope.ScopeConfiguration.Behaviors).CustomEnumerableCreator();

            if (enumerableCreator == null)
            {
                var enumerableType = request.ActivationType.GenericTypeArguments[0];

                var arrayType = enumerableType.MakeArrayType();

                var newRequest = request.NewRequest(arrayType, request.InjectedType, request.RequestType, request.Info, true);

                var arrayExpression = arrayExpressionCreator.GetArrayExpression(scope, newRequest);

                return arrayExpression;
            }
            else
            {
                var enumerableType = request.ActivationType.GenericTypeArguments[0];

                var arrayType = enumerableType.MakeArrayType();

                var newRequest = request.NewRequest(arrayType, request.InjectedType, request.RequestType, request.Info,
                    true);

                var arrayExpression = arrayExpressionCreator.GetArrayExpression(scope, newRequest);

                var enumerableExpression = Expression.Call(Expression.Constant(enumerableCreator),
                    CreateEnumerableMethod.MakeGenericMethod(enumerableType),
                    request.Constants.ScopeParameter,
                    arrayExpression.Expression);

                var returnResult = request.Services.Compiler.CreateNewResult(request, enumerableExpression);

                returnResult.AddExpressionResult(returnResult);

                return returnResult;
            }
        }

        #region CreateEnumerableMethod
        private MethodInfo _createEnumerableMethod;

        protected MethodInfo CreateEnumerableMethod
        {
            get
            {
                return _createEnumerableMethod ??
                       (_createEnumerableMethod = typeof(IEnumerableCreator).GetRuntimeMethods().First());
            }
        }
        #endregion
    }

}
