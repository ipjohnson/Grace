using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.ReadmeGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			ExampleFileProcessor processor = new ExampleFileProcessor();

			foreach (ExampleEntry exampleEntry in processor.ProcessFile("ImportPropertyExamples.cs", ""))
			{
				
			}
		}
	}
}
