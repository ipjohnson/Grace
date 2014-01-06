namespace Grace.UnitTests.Classes.Simple
{
	public interface IExportPropertyService
	{
		IBasicService BasicService { get; }
	}

	public class ExportPropertyService : IExportPropertyService
	{
		private readonly IBasicService basicService = new BasicService();

		public IBasicService BasicService
		{
			get { return basicService; }
		}
	}
}