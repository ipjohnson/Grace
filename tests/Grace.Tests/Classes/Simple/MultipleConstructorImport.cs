namespace Grace.Tests.Classes.Simple
{
    public class MultipleConstructorImport
    {
        public MultipleConstructorImport()
        {
        }

        public MultipleConstructorImport(IBasicService basicService)
        {
            BasicService = basicService;
        }

        public MultipleConstructorImport(IBasicService basicService, IConstructorImportService constructorImportService)
        {
            BasicService = basicService;

            ConstructorImportService = constructorImportService;
        }

        public IBasicService BasicService { get; private set; }

        public IConstructorImportService ConstructorImportService { get; private set; }
    }
}
