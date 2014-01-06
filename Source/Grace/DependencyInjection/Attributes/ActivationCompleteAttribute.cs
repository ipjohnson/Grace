using System;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Methods that are attributed with this class will be called at the end of activation
	/// Note: the signature must be Action() or Action(IInjectionContext)
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class ActivationCompleteAttribute : Attribute, IActivationCompleteAttribute
	{
	}
}