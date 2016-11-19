namespace Grace.Tests.Classes.Simple
{
    public class MethodInjectionClass
    {
        public void InjectBasicService(IBasicService basicService)
        {
            BasicService = basicService;
        }

        public IBasicService BasicService { get; private set; }
    }
}
