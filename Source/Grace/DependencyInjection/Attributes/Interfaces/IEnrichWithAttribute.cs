using System;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributes that implement will be called at discovery time to provide an EnrichWithDelegate
	/// </summary>
	public interface IEnrichWithAttribute
	{
		/// <summary>
		/// Provides an EnrichWithDelegate for an attributed type
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		EnrichWithDelegate ProvideDelegate(Type attributedType);
	}
}