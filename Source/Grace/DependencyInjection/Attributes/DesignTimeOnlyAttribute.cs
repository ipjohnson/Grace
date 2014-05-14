using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Attributed classes will only be used during design time
	/// </summary>
	public class DesignTimeOnlyAttribute : Attribute, IExportEnvironmentAttribute
	{
		public ExportEnvironment ProvideEnvironment(Type attributedType)
		{
			return ExportEnvironment.DesignTimeOnly;
		}
	}
}
