using System;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributes that implement this interface will be queried during discovery to provide a life cycle container
	/// </summary>
	public interface ILifestyleProviderAttribute
	{
		/// <summary>
		/// Provide a Lifestyle container for the attributed type
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <returns></returns>
		ICompiledLifestyle ProvideLifestyle(Type attributedType);
	}
}