using System;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// This attribute can be used to control the priority for the export
	/// Note: ranked high to low
	/// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExportPriorityAttribute : Attribute, IExportPriorityAttribute
	{
		private readonly int _priority;

		/// <summary>
		/// Export priority cosntructor
		/// </summary>
		/// <param name="priority">priority for export, ranked high to low</param>
		public ExportPriorityAttribute(int priority)
		{
			_priority = priority;
		}

		/// <summary>
		/// Provide a priority value
		/// </summary>
		/// <param name="attributedType">type that was attributed</param>
		/// <returns>priority value</returns>
		public int ProvidePriority(Type attributedType)
		{
			return _priority;
		}
	}
}