namespace Grace.UnitTests.Classes.Simple
{
	public class AttributedConstructorMultipleImport
	{
		[SomeTest]
		public AttributedConstructorMultipleImport(ISimpleObject[] simpleObjects)
		{
			Count = simpleObjects.Length;
		}

		public int Count { get; private set; }
	}
}