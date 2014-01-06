using System.Collections.Generic;
using Grace.DependencyInjection.Attributes;
using Grace.UnitTests.Classes.Simple;

namespace Grace.UnitTests.Classes.Attributed
{
	[Export]
	public class AttributedMultipleSimpleObject
	{
		[ImportAttributedWith(typeof(SomeTestAttribute))]
		public IEnumerable<IAttributedSimpleObject> SimpleObjects { get; set; }
	}
}