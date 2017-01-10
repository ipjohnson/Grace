using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Exports attribute with this attribute will be shared as a single instance for all scopes
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class SingletonAttribute : Attribute, ILifestyleProviderAttribute
	{
		/// <summary>
		/// Provide a Lifestyle container for the attributed type
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <returns></returns>
		public ICompiledLifestyle ProvideLifestyle(Type attributedType)
		{
			return new SingletonLifestyle();
		}
	}
}