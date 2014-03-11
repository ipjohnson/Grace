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
		public DirectoryEntry()
		{
			Entries = new List<ClassEntry>();

			Children = new List<DirectoryEntry>();
		}

		public string Title { get; set; }

		public string DirectoryName { get; set; }

		public List<ClassEntry> Entries { get; private set; }

		public DirectoryEntry Parent { get; set; }

		public List<DirectoryEntry> Children { get; private set; }

		public DirectoryEntry RootEntry
		{
			get
			{
				DirectoryEntry returnValue = this;

				while (returnValue.Parent != null)
				{
					returnValue = returnValue.Parent;
				}

				return returnValue;
			}
		}

		public ClassEntry FindEntryByName(string className)
		{
			return InternalFindEntry(this, className);
		}

		private static ClassEntry InternalFindEntry(DirectoryEntry entry, string className)
		{
			ClassEntry returnValue = entry.Entries.FirstOrDefault(x => x.ClassName == className);

			if (returnValue == null)
			{
				foreach (DirectoryEntry directoryEntry in entry.Children)
				{
					returnValue = InternalFindEntry(directoryEntry, className);

					if (returnValue != null)
					{
						break;
					}
				}
			}

			return returnValue;
		}
	}

	public interface IDirectoryProcessor
	{
		DirectoryEntry ProcessDirectory(string directoryPath);
	}

	public class DirectoryProcessor : IDirectoryProcessor
	{
		private ICodeFileProcessor codeFileProcessor;

		public DirectoryProcessor(ICodeFileProcessor codeFileProcessor)
		{
			this.codeFileProcessor = codeFileProcessor;
		}

		public DirectoryEntry ProcessDirectory(string directoryPath)
		{
			string dirName = directoryPath.Replace('/','\\');
			DirectoryEntry directoryEntry = new DirectoryEntry
			                                {
				                                DirectoryName =
																dirName.Remove(0, dirName.LastIndexOf(Path.DirectorySeparatorChar) + 1)
			                                };
			
			foreach (string file in Directory.GetFiles(directoryPath))
			{
				if (file.EndsWith("Example.cs") || file.EndsWith("Examples.cs"))
				{
					directoryEntry.Entries.AddRange(codeFileProcessor.GetClassEntries(file, ClassType.Example));
				}
				else if (file.EndsWith("Module.cs"))
				{
					directoryEntry.Entries.AddRange(codeFileProcessor.GetClassEntries(file, ClassType.Module));
				}
				else
				{
					directoryEntry.Entries.AddRange(codeFileProcessor.GetClassEntries(file, ClassType.Other));
				}
			}

			foreach (string directory in Directory.GetDirectories(directoryPath))
			{
				DirectoryEntry child = ProcessDirectory(directory);

				child.Parent = directoryEntry;

				directoryEntry.Children.Add(child);
			}

			return directoryEntry;
		}
	}
}
