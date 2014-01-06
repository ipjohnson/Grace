using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.VSIXPackageCreater
{
	class Program
	{
		static void Main(string[] args)
		{
			//VSIXPackageCreaterZipArchiver creater = new VSIXPackageCreaterZipArchiver(args[0],args[1],args[2]);
			VSIXPackageCreaterDotNetZip creater = new VSIXPackageCreaterDotNetZip(args[0], args[1], args[2]);


			creater.CreatePackage();
		}
	}
}
