using Grace.DependencyInjection.Attributes;
using Grace.UnitTests.Classes.Simple;

namespace Grace.UnitTests.Classes.Attributed
{
	[SomeTest(TestValue = 5)]
	[Export(typeof(IAttributedSimpleObject))]
	public class AttributedSimpleObjectA : IAttributedSimpleObject
	{
	}

	[SomeTest]
	[Export(typeof(IAttributedSimpleObject))]
	public class AttributedSimpleObjectB : IAttributedSimpleObject
	{
	}

	[SomeTest]
	[Export(typeof(IAttributedSimpleObject))]
	public class AttributedSimpleObjectC : IAttributedSimpleObject
	{
	}

	[Export(typeof(IAttributedSimpleObject))]
	public class AttributedSimpleObjectD : IAttributedSimpleObject
	{
	}

	[Export(typeof(IAttributedSimpleObject))]
	public class AttributedSimpleObjectE : IAttributedSimpleObject
	{
	}
}