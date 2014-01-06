using Grace.DependencyInjection.Attributes;

namespace Grace.UnitTests.Classes.Attributed
{
	public interface IAttributeBasicService
	{
	}

	[Export(typeof(IAttributeBasicService))]
	public class AttributeBasicService : IAttributeBasicService
	{
	}
}