using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Exports under in an Environment
	/// </summary>
	public class ExportEnvironmentAttribute : IExportEnvironmentAttribute
	{
		private ExportEnvironment environment;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="environment">export environment</param>
		public ExportEnvironmentAttribute(ExportEnvironment environment)
		{
			this.environment = environment;
		}

		public ExportEnvironment ProvideEnvironment(Type attributedType)
		{
			return environment;
		}
	}
}
