namespace Grace.UnitTests.Classes.Simple
{
	public class AttributedMethodMultipleImport
	{
		public int Count { get; private set; }

		[SomeTest]
		public void ImportMethod(ISimpleObject[] simpleObjects)
		{
			Count = simpleObjects.Length;
		}
	}
}