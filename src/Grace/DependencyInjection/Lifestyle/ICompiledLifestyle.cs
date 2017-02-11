using System;

namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Represents a lifestyle that can be used for exports
    /// </summary>
    public interface ICompiledLifestyle
    {
        /// <summary>
        /// Clone the lifestyle
        /// </summary>
        /// <returns></returns>
        ICompiledLifestyle Clone();

        /// <summary>
        /// Provide an expression that uses the lifestyle
        /// </summary>
        /// <param name="scope">scope for the strategy</param>
        /// <param name="request">activation request</param>
        /// <param name="activationExpression">expression to create strategy type</param>
        /// <returns></returns>
        IActivationExpressionResult ProvideLifestyleExpression(IInjectionScope scope, IActivationExpressionRequest request, Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression);
    }
}
