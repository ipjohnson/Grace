using Grace.DependencyInjection.Attributes;

namespace Grace.UnitTests.Classes.Attributed
{
	public interface IAttributedOpenGenericTransient<T>
	{
	}

	[Export(typeof(IAttributedOpenGenericTransient<>))]
	public class AttributedOpenGenericTransient<T> : IAttributedOpenGenericTransient<T>
	{
	}
}