using System;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributes that implement this interface will be called during discover to provide a priority
	/// </summary>
	public interface IExportPriorityAttribute
	{
		/// <summary>
		/// Provide the priority for an attributed type
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		int ProvidePriority(Type attributedType);
	}
}