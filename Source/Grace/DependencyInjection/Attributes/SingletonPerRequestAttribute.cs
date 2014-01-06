using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Exports that are marked with this attribute will be shared per request.
	/// Note: request has different meanings in different contexts.
	/// WCF - limited to per WCF operation
	/// MVC - Per HTTP Request
	/// Other - without a context the export will be shared for the injection context
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SingletonPerRequestAttribute : Attribute, ILifestyleProviderAttribute
	{
		/// <summary>
		/// Provide a Lifestyle container for the attributed type
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <returns></returns>
		public ILifestyle ProvideLifestyle(Type attributedType)
		{
			return new SingletonPerRequestLifestyle();
		}
	}
}