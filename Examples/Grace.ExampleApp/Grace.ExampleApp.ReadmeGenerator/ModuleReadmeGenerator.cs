using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.ReadmeGenerator
{
	public interface IModuleReadmeGenerator
	{
		void ProcessDirectoryEntry(DirectoryEntry directoryEntry, string outputPath);
	}

	public class ModuleReadmeGenerator : IModuleReadmeGenerator
	{
		private Func<IExampleReadmeGenerator> exampleGenerator;

		public ModuleReadmeGenerator(Func<IExampleReadmeGenerator> exampleGenerator)
		{
			this.exampleGenerator = exampleGenerator;
		}

		public void ProcessDirectoryEntry(DirectoryEntry directoryEntry, string outputPath)
		{
			ClassEntry modulEntry = directoryEntry.Entries.FirstOrDefault(x => x.ClassType == ClassType.Module);

			GenerateReadmeFile(modulEntry, directoryEntry, outputPath);

			foreach (ClassEntry classEntry in directoryEntry.Entries.FindAll(x => x.ClassType == ClassType.Example))
			{
				IExampleReadmeGenerator readmeGenerator = exampleGenerator();

				readmeGenerator.GenerateReadme(classEntry, directoryEntry, outputPath);
			}

			foreach (DirectoryEntry entry in directoryEntry.Children)
			{
				ModuleReadmeGenerator generator = new ModuleReadmeGenerator(exampleGenerator);

				generator.ProcessDirectoryEntry(entry, outputPath + "\\" + entry.DirectoryName);
			}
		}

		private void GenerateReadmeFile(ClassEntry moduleEntry, DirectoryEntry directoryEntry, string outputPath)
		{
			
		}
	}
}
