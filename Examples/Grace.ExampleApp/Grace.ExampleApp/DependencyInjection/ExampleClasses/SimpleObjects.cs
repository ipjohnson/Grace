namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public interface ISimpleObject
	{
		string SortProperty { get; }
	}

	[SomeTest]
	public class SimpleObjectA : ISimpleObject
	{
		public string SortProperty { get { return "A"; } }
	}

	[SomeTest]
	public class SimpleObjectB : ISimpleObject
	{
		public string SortProperty { get { return "B"; } }
	}

	[SomeTest]
	public class SimpleObjectC : ISimpleObject
	{
		public string SortProperty { get { return "C"; } }
	}

	public class SimpleObjectD : ISimpleObject
	{
		public string SortProperty { get { return "D"; } }
	}

	public class SimpleObjectE : ISimpleObject
	{
		public string SortProperty { get { return "E"; } }
	}
}
