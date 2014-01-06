using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes;

namespace Grace.ExampleApp.DependencyInjection.AttributedExampleClasses
{
	public interface ISomeInterface
	{
		
	}

	[Export(typeof(ISomeInterface))]
	[WhenAppSetting("ISomeInterface","SomeClassA")]
	public class SomeClassA : ISomeInterface
	{
	}

	[Export(typeof(ISomeInterface))]
	[WhenAppSettingNotPresent("ISomeInterface")]
	public class SomeClassB : ISomeInterface
	{
	}
}
