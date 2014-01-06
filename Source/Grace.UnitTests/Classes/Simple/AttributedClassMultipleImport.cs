using System.Collections.Generic;

namespace Grace.UnitTests.Classes.Simple
{
	[SomeTest]
	public class AttributedClassMultipleImport
	{
		public AttributedClassMultipleImport(ISimpleObject[] simpleObjects)
		{
			SimpleObjects = simpleObjects;
		}

		public IEnumerable<ISimpleObject> SimpleObjects { get; private set; }
	}
}