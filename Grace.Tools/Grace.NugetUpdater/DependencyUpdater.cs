using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.NugetUpdater
{
	public class DependencyUpdater
	{
		private string relativePath;
		private string versionString;

		public DependencyUpdater(string relativePath, string versionString)
		{
			this.relativePath = relativePath;
			this.versionString = versionString;
		}

		public void Update()
		{
			UpdateGraceVersion();
		}

		private void UpdateGraceVersion()
		{
			string nugetPath = Directory.GetCurrentDirectory() + relativePath;

			Console.WriteLine("Updating nuget path " + nugetPath);

			foreach (string file in Directory.GetFiles(nugetPath, "*.nuspec"))
			{
				IEnumerable<string> lines = File.ReadAllLines(file);
				string replaceFile = null;
				bool foundGraceLine = false;

				foreach (string line in lines)
				{
					if (line.Contains("<dependency id=\"Grace\" version=\""))
					{
						int lessThanIndex = line.IndexOf('<');
						foundGraceLine = true;
						replaceFile += string.Format("{0}<dependency id=\"Grace\" version=\"{1}\"/>{2}",
															  line.Substring(0, lessThanIndex),
															  versionString,
															  Environment.NewLine);
					}
					else
					{
						replaceFile += line + Environment.NewLine;
					}
				}

				if (foundGraceLine)
				{
					File.WriteAllText(file, replaceFile);
				}
			}
		}
	}
}
