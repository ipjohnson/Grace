using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents a decortator or an export
    /// </summary>
    public interface IDecoratorOrExportActivationStrategy : IActivationStrategy
    {
        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle);
    }
}
