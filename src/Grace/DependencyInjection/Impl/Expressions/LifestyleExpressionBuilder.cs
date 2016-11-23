using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// interface for building expressions for a activation type using lifestyles
    /// </summary>
    public interface ILifestyleExpressionBuilder
    {
        /// <summary>
        /// Type expression builder
        /// </summary>
        ITypeExpressionBuilder TypeExpressionBuilder { get; }

        /// <summary>
        /// Get activation expression for type configuration
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="request">request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <param name="lifestyle">lifestyle</param>
        /// <returns></returns>
        IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, ICompiledLifestyle lifestyle);
    }

    /// <summary>
    /// class builds expressions for activation configurations using lifestyles
    /// </summary>
    public class LifestyleExpressionBuilder : ILifestyleExpressionBuilder
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="typeExpressionBuilder"></param>
        public LifestyleExpressionBuilder(ITypeExpressionBuilder typeExpressionBuilder)
        {
            TypeExpressionBuilder = typeExpressionBuilder;
        }

        /// <summary>
        /// Type expression builder
        /// </summary>
        public ITypeExpressionBuilder TypeExpressionBuilder { get; }

        /// <summary>
        /// Get activation expression for type configuration
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="request">request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <param name="lifestyle">lifestyle</param>
        /// <returns></returns>
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
