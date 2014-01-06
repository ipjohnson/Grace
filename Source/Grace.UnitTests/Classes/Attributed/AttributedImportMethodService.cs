using Grace.DependencyInjection.Attributes;

namespace Grace.UnitTests.Classes.Attributed
{
	public interface IAttributedImportMethodService
	{
		IAttributeBasicService BasicService { get; }
	}

	[Export(typeof(IAttributedImportMethodService))]
	public class AttributedImportMethodService : IAttributedImportMethodService
	{
		public IAttributeBasicService BasicService { get; private set; }

		[Import]
		public void ImportMethod(IAttributeBasicService basicService)
		{
			BasicService = basicService;
		}
	}
}