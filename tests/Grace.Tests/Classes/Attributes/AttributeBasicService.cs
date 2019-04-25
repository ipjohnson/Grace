using Grace.DependencyInjection.Attributes;

namespace Grace.Tests.Classes.Attributes
{
    public interface IAttributeBasicService
    {
    }

    [Metadata(123, "Hello")]
    [Metadata(456, "World")]
    [Export(typeof(IAttributeBasicService))]
    public class AttributeBasicService : IAttributeBasicService
    {
    }
}