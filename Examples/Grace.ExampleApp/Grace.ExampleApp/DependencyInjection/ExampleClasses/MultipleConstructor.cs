using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public class MultipleConstructor
	{
		public MultipleConstructor(IBasicService basicService)
		{
			BasicService = basicService;
		}

		public MultipleConstructor(IBasicService basicService, ISimpleObject simpleObject)
		{
			BasicService = basicService;

			SimpleObject = simpleObject;
		}

		public IBasicService BasicService { get; private set; }

		public ISimpleObject SimpleObject { get; private set; }
	}
}
