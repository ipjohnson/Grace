namespace Grace.Tests.Classes.Simple
{
    public interface IPropertyInjectionService
    {
        IBasicService BasicService { get; }
    }

    public class PropertyInjectionService : IPropertyInjectionService
    {
        public IBasicService BasicService { get; set; }
    }
}
