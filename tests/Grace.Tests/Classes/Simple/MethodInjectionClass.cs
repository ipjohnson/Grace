namespace Grace.Tests.Classes.Simple
{
    public class MethodInjectionClass
    {
        public void InjectMethod(IBasicService basicService)
        {
            BasicService = basicService;
        }

        public void SomeOtherMethod(IBasicService basicService)
        {
            SecondService = basicService;
        }

        public IBasicService BasicService { get; set; }

        public IBasicService SecondService { get; set; }
    }
}
