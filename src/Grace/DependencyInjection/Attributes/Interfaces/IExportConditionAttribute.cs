using System;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributes that implement this interface can provide a condition for export
	/// </summary>
	public interface IExportConditionAttribute
	{
		/// <summary>
		/// Provide an export condition for an attirbuted type
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		ICompiledCondition ProvideCondition(Type attributedType);
	}
}