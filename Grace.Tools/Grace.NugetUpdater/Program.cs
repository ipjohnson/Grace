using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.NugetUpdater
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				throw new ArgumentException("number of args should be 2");
			}

			DependencyUpdater updater = new DependencyUpdater(args[0],args[1]);

			updater.Update();
		}
	}
}
