namespace Grace.Tests.Classes.Simple
{
    public interface IImportOneInstanceOfMultipleService
    {
        IMultipleService MultipleService { get; }    
    }

    public class ImportOneInstanceOfMultipleService : IImportOneInstanceOfMultipleService
    {
        public ImportOneInstanceOfMultipleService(IMultipleService multipleService)
        {
            MultipleService = multipleService;
        }

        public IMultipleService MultipleService { get; }
    }
}
