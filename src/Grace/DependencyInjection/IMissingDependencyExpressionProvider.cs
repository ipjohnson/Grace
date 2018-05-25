using System;
using System.Collections.Generic;
using System.Text;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Interface for providing linq expressions for missing dependencies
    /// </summary>
    public interface IMissingDependencyExpressionProvider
    {
        /// <summary>
        /// Provide linq expression for the missing dependency
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns>return expression or null if it's not possible</returns>
        IActivationExpressionResult ProvideExpression(IInjectionScope scope,
            IActivationExpressionRequest request);
    }
}
