using Grace.Tests.Classes.Simple;

namespace Grace.Tests.Classes.Attributes
{
    public interface IAttributedImportHasAttributeService
    {
        IMultipleService MultipleService { get; }
    }

    [Test]
    public class AttributedImportHasAttributeService : IAttributedImportHasAttributeService
    {
        public AttributedImportHasAttributeService(IMultipleService multipleService)
        {
            MultipleService = multipleService;
        }

        public IMultipleService MultipleService { get; }
    }

    public interface IAttributedImportNoAttributeService
    {
        IMultipleService MultipleService { get; }
    }

    public class AttributedImportNoAttributeService : IAttributedImportNoAttributeService
    {
        public AttributedImportNoAttributeService(IMultipleService multipleService)
        {
            MultipleService = multipleService;
        }

        public IMultipleService MultipleService { get; }
    }
}
