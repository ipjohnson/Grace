using Grace.DependencyInjection.Attributes;

namespace Grace.Tests.Classes.Attributes
{
	public interface IAttributeBasicService
	{
	}

	[Export(typeof(IAttributeBasicService))]
	public class AttributeBasicService : IAttributeBasicService
	{
	}
}