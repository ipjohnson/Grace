namespace Grace.Tests.Classes.Simple
{
    public interface IConstructorImportService
    {
        IBasicService BasicService { get; }
    }

    public class ConstructorImportService : IConstructorImportService
    {
        public ConstructorImportService(IBasicService basicService)
        {
            BasicService = basicService;
        }

        public IBasicService BasicService { get; }
    }
}
