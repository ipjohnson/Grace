using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.ExampleApp.ReadmeGenerator
{
	class Program
	{
		private static string BaseUrl =
			"https://github.com/ipjohnson/Grace/tree/master/Examples/Grace.ExampleApp.Documentation";

		private static string BaseProjectDir = "../../../Grace.ExampleApp/";
		private static string BaseOutputDir = "../../../../Grace.ExampleApp.Documentation/";

		static void Main(string[] args)
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()).ByInterfaces());

			var context = container.CreateContext();

			context.Export("baseUrl", (s, c) => BaseUrl);

			var projectProcessor = container.Locate<IProjectReadmeGenerator>(context);

			projectProcessor.GenerateReadme(BaseOutputDir, BaseProjectDir + "DependencyInjection/");
		}
	}
}
