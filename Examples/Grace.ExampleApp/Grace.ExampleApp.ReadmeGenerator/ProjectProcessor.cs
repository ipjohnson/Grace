using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.ReadmeGenerator
{
	public interface IProjectReadmeGenerator
	{
		void GenerateReadme(string outputPath, params string[] projectDirectories);
	}

	public class ProjectProcessor : IProjectReadmeGenerator
	{
		private IDirectoryProcessor directoryProcessor;
		private Func<IModuleReadmeGenerator> moduleGenerator;

		public ProjectProcessor(IDirectoryProcessor directoryProcessor,Func<IModuleReadmeGenerator> moduleGenerator)
		{
			this.directoryProcessor = directoryProcessor;
			this.moduleGenerator = moduleGenerator;
		}

		public void GenerateReadme(string outputPath, params string[] projectDirectories)
		{
			foreach (string projectDirectory in projectDirectories)
			{
				DirectoryEntry directoryEntry = directoryProcessor.ProcessDirectory(projectDirectory);

				IModuleReadmeGenerator moduleReadmeGenerator = moduleGenerator();

				moduleReadmeGenerator.ProcessDirectoryEntry(directoryEntry,outputPath + directoryEntry.DirectoryName);
			}
		}
	}
}
