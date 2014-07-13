using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.ReadmeGenerator
{
	public interface IExampleReadmeGenerator
	{
		void GenerateReadme(ClassEntry classEntry, DirectoryEntry directoryEntry, string outputPath);
	}

	public class ExampleReadmeGenerator : IExampleReadmeGenerator
	{
		private string baseUrl;
		private FileStream outputStream;

		public ExampleReadmeGenerator(string baseUrl)
		{
			this.baseUrl = baseUrl;
		}


		public void GenerateReadme(ClassEntry classEntry, DirectoryEntry directoryEntry, string outputPath)
		{
			OpenFile(outputPath + "\\" + classEntry.ClassName + ".md");





			CloseFile();
		}

		private void OpenFile(string fileName)
		{
			string directoryPath = Path.GetDirectoryName(fileName);

			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			outputStream = File.OpenWrite(fileName);
		}

		private void CloseFile()
		{
			outputStream.Close();
			outputStream.Dispose();
		}
	}
}
