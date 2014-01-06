namespace Grace.UnitTests.Classes.Simple
{
	public interface IMultiplePropertyImportService
	{
		IImportConstructorService ConstructorService { get; }

		IImportMethodService MethodService { get; }

		IImportPropertyService PropertyService { get; }
	}

	public class MultiplePropertyImportService : IMultiplePropertyImportService
	{
		public IImportConstructorService ConstructorService { get; set; }

		public IImportMethodService MethodService { get; set; }

		public IImportPropertyService PropertyService { get; set; }
	}
}