using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.ReadmeGenerator
{
	public class DirectoryEntry
	{
		public string Title { get; set; }
	}

	public interface IDirectoryProcessor
	{
		DirectoryEntry ProcessDirectory(string directoryPath, string outputPath);
	}

	public class DirectoryProcessor : IDirectoryProcessor
	{
		private IExampleFileProcessor exampleFileProcessor;
		private IModuleFileProcessor moduleFileProcessor;
		private Func<IDirectoryProcessor> directoryFactory;

		public DirectoryProcessor(Func<IDirectoryProcessor> directoryFactory,
										  IExampleFileProcessor exampleFileProcessor,
										  IModuleFileProcessor moduleFileProcessor)
		{
			this.directoryFactory = directoryFactory;
			this.exampleFileProcessor = exampleFileProcessor;
			this.moduleFileProcessor = moduleFileProcessor;
		}

		public DirectoryEntry ProcessDirectory(string directoryPath, string outputPath)
		{
			DirectoryEntry directoryEntry = new DirectoryEntry();
			
			foreach (string file in Directory.GetFiles(directoryPath))
			{
				if (file.EndsWith("Example.cs"))
				{
					exampleFileProcessor.ProcessFile(file, outputPath);
				}
				else if (file.EndsWith("Module.cs"))
				{
					
				}
			}

			foreach (string directory in Directory.GetDirectories(directoryPath))
			{
				
			}

			return directoryEntry;
		}
	}
}
