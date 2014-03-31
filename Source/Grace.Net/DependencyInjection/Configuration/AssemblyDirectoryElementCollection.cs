using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// Loads assemblies from a directory
	/// </summary>
	public class AssemblyDirectoryElementCollection : BaseElementCollection<AssemblyDirectoryElement>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public AssemblyDirectoryElementCollection() : base("directory")
		{

		}
	}
}
