using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// Cofiguration object that loads all assemblies from a directory
	/// </summary>
	public class AssemblyDirectoryElement : BaseElement
	{
		/// <summary>
		/// Scan for types that have been attributed in assembly
		/// </summary>
		[ConfigurationProperty("scanForAttributes", DefaultValue = false, IsRequired = false)]
		public bool ScanForAttributes
		{
			get { return PropertyValue<bool>(); }
		}
		
		/// <summary>
		/// Path to assembly file
		/// </summary>
		[ConfigurationProperty("path", IsRequired = true)]
		public string Path
		{
			get { return PropertyValue<string>(); }
		}

		/// <summary>
		/// Exported interfaces
		/// </summary>
		[ConfigurationProperty("exportInterfaces", IsRequired = false)]
		public ExportInterfaceElementCollection ExportInterfaces
		{
			get { return PropertyValue<ExportInterfaceElementCollection>(); }
		}
	}
}
