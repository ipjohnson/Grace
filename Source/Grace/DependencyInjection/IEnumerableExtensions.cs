using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Extensions for IEnumerable for dependency injection
	/// </summary>
   // ReSharper disable once InconsistentNaming
	public static class IEnumerableExtensions
	{
		/// <summary>
		/// Extension to export a list of types to a registration block
		/// </summary>
		/// <param name="types">list of types</param>
		/// <param name="registrationBlock">registration block</param>
		/// <returns>configuration object</returns>
		public static IExportTypeSetConfiguration ExportTo(this IEnumerable<Type> types, IExportRegistrationBlock registrationBlock)
		{
			return registrationBlock.Export(types);
		}
	}
}
