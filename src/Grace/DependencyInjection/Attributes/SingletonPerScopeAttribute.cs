using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Exports marked with this attribute will be shared per scope.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class SingletonPerScopeAttribute : Attribute, ILifestyleProviderAttribute
	{
		/// <summary>
		/// Provide a Lifestyle container for the attributed type
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <returns></returns>
		public ICompiledLifestyle ProvideLifestyle(Type attributedType)
		{
			return new SingletonPerScopeLifestyle(false);
		}
	}
}