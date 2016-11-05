using Grace.DependencyInjection.Attributes;

namespace Grace.Tests.Classes.Attributes
{
	public interface IAttributedImportPropertyService
	{
		IAttributeBasicService BasicService { get; }
	}

	[Export(typeof(IAttributedImportPropertyService))]
	public class AttributedImportPropertyService : IAttributedImportPropertyService
	{
		[Import]
		public IAttributeBasicService BasicService { get; set; }
	}
}