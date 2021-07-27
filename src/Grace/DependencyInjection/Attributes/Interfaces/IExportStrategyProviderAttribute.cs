using System;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributes that implement this interface will be called at discovery time to provide an export strategy
	/// </summary>
	public interface IExportStrategyProviderAttribute
	{
        /// <summary>
        /// Provide an export strategy for the attributed type
        /// </summary>
        /// <param name="attributedType"></param>
        /// <param name="activationStrategyCreator"></param>
        /// <returns></returns>
        ICompiledExportStrategy ProvideStrategy(Type attributedType,
            IActivationStrategyCreator activationStrategyCreator);
	}
}