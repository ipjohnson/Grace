using System;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	public class ExportPriorityAttribute : Attribute, IExportPriorityAttribute
	{
		private readonly int priority;

		public ExportPriorityAttribute(int priority)
		{
			this.priority = priority;
		}

		public int ProvidePriority(Type attributedType)
		{
			return priority;
		}
	}
}