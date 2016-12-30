using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Exports marked with this attribute will be shared per injection context
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class SingletonPerObjectGraphAttribute : Attribute, ILifestyleProviderAttribute
	{
		/// <summary>
		/// Provide a Lifestyle container for the attributed type
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <returns></returns>
		public ICompiledLifestyle ProvideLifestyle(Type attributedType)
		{
			return new SingletonPerObjectGraph(false);
		}
	}
}