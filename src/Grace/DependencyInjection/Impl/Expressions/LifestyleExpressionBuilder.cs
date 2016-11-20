using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public interface ILifestyleExpressionBuilder
    {
        ITypeExpressionBuilder TypeExpressionBuilder { get; }

        IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, ICompiledLifestyle lifestyle);
    }

    public class LifestyleExpressionBuilder : ILifestyleExpressionBuilder
    {
        public LifestyleExpressionBuilder(ITypeExpressionBuilder typeExpressionBuilder)
        {
            TypeExpressionBuilder = typeExpressionBuilder;
        }

        public ITypeExpressionBuilder TypeExpressionBuilder { get; }

        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, ICompiledLifestyle lifestyle)
        {
            return StandardActivationExpression(scope, request, activationConfiguration, lifestyle);
        }

        private IActivationExpressionResult StandardActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, ICompiledLifestyle lifestyle)
        {
            if (lifestyle != null && lifestyle.RootRequest)
            {
                var compilerService = request.Services.Compiler;

                request = compilerService.CreateNewRequest(request.ActivationType, request.ObjectGraphDepth + 1, scope);
            }

            var currentExpression = TypeExpressionBuilder.GetActivationExpression(scope, request, activationConfiguration);

            if (currentExpression == null)
            {
                return null;
            }

            if (lifestyle != null)
            {
                currentExpression = lifestyle.ProvideLifestlyExpression(scope, request, currentExpression);
            }

            return currentExpression;
        }
    }
}
