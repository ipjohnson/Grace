using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Defines properties that are specific to the property that is being imported
	/// </summary>
	public interface IInjectionTargetInfo
	{
		/// <summary>
		/// This is the type that is being injected into 
		/// </summary>
		[NotNull]
		Type InjectionType { get; }

		/// <summary>
		/// These are the attributes for the class that it's being injected into
		/// </summary>
		[NotNull]
		IEnumerable<Attribute> InjectionTypeAttributes { get; }

		/// <summary>
		/// The PropertyInfo or ParameterInfo that is being injected
		/// </summary>
		[NotNull]
		object InjectionTarget { get; }

		/// <summary>
		/// This is the property or parameter name being injected
		/// </summary>
		[NotNull]
		string InjectionTargetName { get; }

		/// <summary>
		/// This the type for the Property or Parameter being injected
		/// </summary>
		[NotNull]
		Type InjectionTargetType { get; }

		/// <summary>
		/// Attributes associated with the target PropertyInfo or ParameterInfo that is provided
		/// </summary>
		[NotNull]
		IEnumerable<Attribute> InjectionTargetAttributes { get; }

		/// <summary>
		/// Attributes on the Constructor, Method, or Property being injected
		/// </summary>
		[NotNull]
		IEnumerable<Attribute> InjectionMemberAttributes { get; }

		/// <summary>
		/// Locate name being used
		/// </summary>
		string LocateName { get; }

		/// <summary>
		/// Locate type being used
		/// </summary>
		Type LocateType { get; }
	}
}