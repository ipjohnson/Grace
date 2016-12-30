using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Exports attributed with this will be shared, the instance will be held with a weak reference so it will be GC'd when done
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class WeakSingletonAttribute : Attribute, ILifestyleProviderAttribute
	{
		/// <summary>
		/// Provide a Lifestyle container for the attributed type
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <returns></returns>
		public ICompiledLifestyle ProvideLifestyle(Type attributedType)
		{
			return new WeakSingletonLifestyle();
		}
	}
}