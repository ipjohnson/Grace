using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// interface for building expressions for a activation type using lifestyles
    /// </summary>
    public interface IDefaultStrategyExpressionBuilder
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
    public class DefaultStrategyExpressionBuilder : IDefaultStrategyExpressionBuilder
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="typeExpressionBuilder"></param>
        public DefaultStrategyExpressionBuilder(ITypeExpressionBuilder typeExpressionBuilder)
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
            if (lifestyle == null)
            {
                return TypeExpressionBuilder.GetActivationExpression(scope, request, activationConfiguration);
            }

            return lifestyle.ProvideLifestyleExpression(scope, request,
                lifestyleRequest => TypeExpressionBuilder.GetActivationExpression(scope, lifestyleRequest, activationConfiguration));
        }
    }
}
