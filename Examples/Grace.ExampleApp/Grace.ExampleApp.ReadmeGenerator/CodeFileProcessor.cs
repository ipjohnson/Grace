using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.ReadmeGenerator
{
	public enum ClassType
	{
		Example,
		Module,
		Other,
	}

	public class ClassEntry
	{
		public string ClassName { get; set; }

		public string Description { get; set; }

		public string Class { get; set; }

		public ClassType ClassType { get; set; }
	}

	public interface ICodeFileProcessor
	{
		IEnumerable<ClassEntry> GetClassEntries(string filePath, ClassType classType);
	}

	public class CodeFileProcessor : ICodeFileProcessor
	{
		public IEnumerable<ClassEntry> GetClassEntries(string filePath, ClassType classType)
		{
			List<string> lines = new List<string>(File.ReadAllLines(filePath));

			RemoveUsingAndNamspace(lines);

			int classIndex = lines.FindLastIndex(x => x.Contains("public class "));

			while (classIndex >= 0)
			{
				yield return ProcessExampleClass(lines, classIndex, classType);

				classIndex = lines.FindLastIndex(x => x.Contains("public class "));
			}
		}

		private ClassEntry ProcessExampleClass(List<string> lines, int classIndex, ClassType classType)
		{
			ClassEntry returnEntry = new ClassEntry
			{
				ClassName = GetClassName(lines[classIndex]),
				ClassType = classType
			};

			int commentBeginIndex = classIndex - 1;

			while (commentBeginIndex >= 0 && lines[commentBeginIndex].Contains("///"))
			{
				commentBeginIndex--;
			}

			commentBeginIndex++;

			string commentString = "";

			for (int i = commentBeginIndex; i < classIndex; i++)
			{
				if (lines[i].Contains("<summary>") || lines[i].Contains("</summary>"))
				{
					continue;
				}

				commentString += lines[i].TrimStart(' ', '/', '\t') + Environment.NewLine;
			}

			returnEntry.Description = commentString;

			StringBuilder stringBuilder = new StringBuilder();

			for (int i = classIndex; i < lines.Count; i++)
			{
				stringBuilder.AppendLine(lines[i]);
			}

			returnEntry.Class = stringBuilder.ToString();

			lines.RemoveRange(commentBeginIndex, lines.Count - commentBeginIndex);

			return returnEntry;
		}

		private string GetClassName(string classString)
		{
			int classIndex = classString.IndexOf("class ");
			string returnValue = classString;

			returnValue = returnValue.Substring(classIndex + 6);

			int space = returnValue.IndexOf(' ');

			if (space > 0)
			{
				returnValue = returnValue.Substring(0, space);
			}

			return returnValue;
		}

		private void RemoveUsingAndNamspace(List<string> fileLines)
		{
			fileLines.RemoveAll(x => x.StartsWith("using "));

			RemoveBlankLinesFromFront(fileLines);

			fileLines.RemoveRange(0, 2);

			fileLines.RemoveAt(fileLines.Count - 1);
		}

		private void RemoveBlankLinesFromFront(List<string> lines)
		{
			while (lines.Count > 0)
			{
				if (lines[0] == string.Empty)
				{
					lines.RemoveAt(0);
				}
				else
				{
					break;
				}
			}
		}
	}
}
