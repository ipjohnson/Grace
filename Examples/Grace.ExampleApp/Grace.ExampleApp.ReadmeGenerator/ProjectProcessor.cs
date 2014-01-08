using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.ReadmeGenerator
{
	public interface IReadmeGenerator
	{
		void GenerateReadme();
	}

	public class ProjectProcessor : IReadmeGenerator
	{
		public void GenerateReadme()
		{
			
		}
	}
}
