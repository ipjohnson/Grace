using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Wraps an export activation delegate to use as a value provider
	/// </summary>
	public class ExportActivationValueProvider : IExportValueProvider
	{
		private readonly ExportActivationDelegate activationDelegate;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="activationDelegate">activation delegate</param>
		public ExportActivationValueProvider(ExportActivationDelegate activationDelegate)
		{
			this.activationDelegate = activationDelegate;
		}

		/// <summary>
		/// Activate the export
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
		{
			return activationDelegate(exportInjectionScope, context);
		}
	}
}
