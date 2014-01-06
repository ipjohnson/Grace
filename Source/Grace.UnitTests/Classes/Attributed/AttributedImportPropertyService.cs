using Grace.DependencyInjection.Attributes;

namespace Grace.UnitTests.Classes.Attributed
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