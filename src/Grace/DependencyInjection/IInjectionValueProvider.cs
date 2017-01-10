namespace Grace.DependencyInjection
{
    /// <summary>
    /// Interface to provide expression for when a request is made
    /// </summary>
    public interface IInjectionValueProvider
    {
        /// <summary>
        /// Get an expression for the request, returns null if this provider doesn't support it
        /// </summary>
        /// <param name="scope">scope for request</param>
        /// <param name="request">request for expression</param>
        /// <returns>expression result</returns>
        IActivationExpressionResult GetExpressionResult(IInjectionScope scope, IActivationExpressionRequest request);
    }
}
