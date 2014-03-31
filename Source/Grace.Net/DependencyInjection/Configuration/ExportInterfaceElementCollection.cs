using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// Configuration element to export interfaces 
	/// </summary>
	public class ExportInterfaceElementCollection : BaseElementCollection<ExportInterfaceElement>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ExportInterfaceElementCollection() : base("exportInterface")
		{

		}
	}
}
