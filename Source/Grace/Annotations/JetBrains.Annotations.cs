using System;

#pragma warning disable 1591
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace
namespace JetBrains.Annotations
{
	/// <summary>
	/// Indicates that the value of the marked element could be <c>null</c> sometimes,
	/// so the check for <c>null</c> is necessary before its usage
	/// </summary>
	/// <example><code>
	/// [CanBeNull] public object Test() { return null; }
	/// public void UseTest() {
	///   var p = Test();
	///   var s = p.ToString(); // Warning: Possible 'System.NullReferenceException'
	/// }
	/// </code></example>
	[AttributeUsage(
	  AttributeTargets.Method | AttributeTargets.Parameter |
	  AttributeTargets.Property | AttributeTargets.Delegate |
	  AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	internal sealed class CanBeNullAttribute : Attribute { }

	/// <summary>
	/// Indicates that the value of the marked element could never be <c>null</c>
	/// </summary>
	/// <example><code>
	/// [NotNull] public object Foo() {
	///   return null; // Warning: Possible 'null' assignment
	/// }
	/// </code></example>
	[AttributeUsage(
	  AttributeTargets.Method | AttributeTargets.Parameter |
	  AttributeTargets.Property | AttributeTargets.Delegate |
	  AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	internal sealed class NotNullAttribute : Attribute { }

	/// <summary>
	/// Indicates that the marked method builds string by format pattern and (optional) arguments.
	/// Parameter, which contains format string, should be given in constructor. The format string
	/// should be in <see cref="string.Format(IFormatProvider,string,object[])"/>-like form
	/// </summary>
	/// <example><code>
	/// [StringFormatMethod("message")]
	/// public void ShowError(string message, params object[] args) { /* do something */ }
	/// public void Foo() {
	///   ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
	/// }
	/// </code></example>
	[AttributeUsage(
		AttributeTargets.Constructor | AttributeTargets.Method,
		AllowMultiple = false, Inherited = true)]
	internal sealed class StringFormatMethodAttribute : Attribute
	{
		/// <param name="formatParameterName">
		/// Specifies which parameter of an annotated method should be treated as format-string
		/// </param>
		public StringFormatMethodAttribute(string formatParameterName)
		{
			FormatParameterName = formatParameterName;
		}

		public string FormatParameterName { get; private set; }
	}

}
