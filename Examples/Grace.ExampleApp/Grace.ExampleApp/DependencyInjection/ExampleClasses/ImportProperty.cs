using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public interface IImportProperty
	{
		IBasicService BasicService { get; }	
	}

	public class ImportProperty : IImportProperty
	{
		public IBasicService BasicService { get; set; }
	}
}
