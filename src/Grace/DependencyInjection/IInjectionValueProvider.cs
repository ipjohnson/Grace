namespace Grace.DependencyInjection
{
    /// <summary>
    /// Interface to provide expression for when a request is made
    /// </summary>
    public interface IInjectionValueProvider
    {
        /// <summary>
        ///  Can a value be located for this request
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        bool CanLocate(IInjectionScope scope, IActivationExpressionRequest request);

        /// <summary>
        /// Get an expression for the request, returns null if this provider doesn't support it
        /// </summary>
        /// <param name="scope">scope for request</param>
        /// <param name="request">request for expression</param>
        /// <returns>expression result</returns>
        IActivationExpressionResult GetExpressionResult(IInjectionScope scope, IActivationExpressionRequest request);
    }
}
