namespace Grace.Tests.Classes.Simple
{
    public class ImportKeyService
    {
        public ImportKeyService()
        { }

        public ImportKeyService(object key)
        { 
            ObjectKey = key;
        }

        public void ImportMethod(object key)
        { 
            ObjectKey = key;
        }

        public object ObjectKey { get; set; }
        public string StringKey { get; set; }
        public int IntKey { get; set; }
    }

    public class ImportKeyServiceWrapper
    {
        public ImportKeyServiceWrapper(ImportKeyService service)
        {
            Service = service;
        }

        public ImportKeyService Service { get; }

        public object ObjectKey { get; set; }
    }
}