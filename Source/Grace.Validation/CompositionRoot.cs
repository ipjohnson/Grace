using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.Validation
{
	/// <summary>
	/// This class can be used to boot strap the validation engine into the container
	/// </summary>
	public class CompositionRoot : IConfigurationModule
	{
		/// <summary>
		/// Use the validation attributes provided in the Attributes namespace
		/// </summary>
		public bool UseAttributes { get; set; }

		public void Configure(IExportRegistrationBlock registrationBlock)
		{
			
		}
	}
}
