using Grace.DependencyInjection.Attributes;

namespace Grace.Tests.Classes.Attributes
{
    public interface IAttributedExportService
    {
        IAttributeBasicService BasicService { get; }
    }

    [Export(typeof(IAttributedExportService))]
    public class AttributedExportService : IAttributedExportService
    {
        [Export]
        public IAttributeBasicService BasicService
        {
            get
            {
                return new AttributeBasicService(); 
            }
        }
    }
}
