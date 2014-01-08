using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.ReadmeGenerator
{
	public interface IModuleFileProcessor
	{
		void ProcessFile(string filePath, string outputFilePath);
	}

	public class ModuleFileProcessor
	{

	}
}
