using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public interface IDisposalScopeExpressionCreator
    {
        IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result);
    }

    public class DisposalScopeExpressionCreator : IDisposalScopeExpressionCreator
    {
        private MethodInfo _addMethod;

        public IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result)
        {
            var closedGeneric = AddMethod.MakeGenericMethod(activationConfiguration.ActivationType);

            var closedActionType = typeof(Action<>).MakeGenericType(activationConfiguration.ActivationType);

            object disposalDelegate = null;

            if (closedActionType == activationConfiguration?.DisposalDelegate.GetType())
            {
                disposalDelegate = activationConfiguration.DisposalDelegate;
            }

            var disposalCall = 
                Expression.Call(request.DisposalScopeExpression, closedGeneric, result.Expression, Expression.Convert(Expression.Constant(disposalDelegate), closedActionType));

            return request.Services.Compiler.CreateNewResult(request, disposalCall);
        }

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
