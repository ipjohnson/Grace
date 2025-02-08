using Grace.DependencyInjection.Attributes;
using Grace.Tests.Classes.Simple;

namespace Grace.Tests.Classes.Attributes
{
    public class AttributedConstructorService
    {
        [Import]
        public AttributedConstructorService(IBasicService basicService)
        {
            BasicService = basicService;
        }

        public AttributedConstructorService(IBasicService basicService, IMultipleService multipleService)
        {
            BasicService = basicService;
            MultipleService = multipleService;
        }

        public IBasicService BasicService { get; }

        public IMultipleService MultipleService { get; }

    }

    public class AdaptedAttributedConstructorService
    {
        [AdaptedImport]
        public AdaptedAttributedConstructorService(IBasicService basicService)
        {
            BasicService = basicService;
        }

        public AdaptedAttributedConstructorService(IBasicService basicService, IMultipleService multipleService)
        {
            BasicService = basicService;
            MultipleService = multipleService;
        }

        public IBasicService BasicService { get; }

        public IMultipleService MultipleService { get; }
    }
}
