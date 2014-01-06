using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Grace.DependencyInjection;

namespace Grace.MVC.DependencyInjection
{
	/// <summary>
	/// C# extension class for registering different MVC types
	/// </summary>
	public static class IExportRegistrationBlockExtensions
	{
		/// <summary>
		/// C# extension that exports all the classes inheriting from Controller
		/// </summary>
		/// <param name="registrationBlock"></param>
		/// <param name="types"></param>
		public static void ExportController(this IExportRegistrationBlock registrationBlock, IEnumerable<Type> types)
		{
			registrationBlock.Export(types).
				BasedOn(typeof(Controller)).
				ExternallyOwned();
		}
	}
}