using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	public class ExportPriorityAttribute : Attribute, IExportPriorityAttribute
	{
		private int priority;

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
