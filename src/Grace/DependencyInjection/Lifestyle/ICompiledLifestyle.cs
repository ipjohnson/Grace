namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Represents a lifestyle that can be used for exports
    /// </summary>
    public interface ICompiledLifestyle
    {
        /// <summary>
        /// Root the request context when creating expression
        /// </summary>
        bool RootRequest { get; }

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
        IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope, IActivationExpressionRequest request, IActivationExpressionResult activationExpression);
    }
}
