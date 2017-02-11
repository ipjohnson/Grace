using System;

namespace Grace.DependencyInjection.Lifestyle
{

    /// <summary>
    /// Generalized type for lifestyle
    /// </summary>
    public enum LifestyleType
    {
        /// <summary>
        /// Lifestyle is roughly transient (PerObjectGraph, PerAncestorType)
        /// </summary>
        Transient,

        /// <summary>
        /// Singleton that is tied to a scope
        /// </summary>
        Scoped,

        /// <summary>
        /// Singleton for the whole container
        /// </summary>
        Singleton
    }

    /// <summary>
    /// Represents a lifestyle that can be used for exports
    /// </summary>
    public interface ICompiledLifestyle
    {
        /// <summary>
        /// Generalization for lifestyle
        /// </summary>
        LifestyleType LifestyleType { get; }

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
