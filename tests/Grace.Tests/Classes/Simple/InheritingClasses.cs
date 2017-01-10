namespace Grace.Tests.Classes.Simple
{
    public interface ICustomBaseService
    {
        int SomeValue { get; }
    }

    public interface IInheritingService : ICustomBaseService
    {
        
    }

    public class CustomBaseService : ICustomBaseService
    {
        public int SomeValue => 5;
    }

    public class InheritingClasses : CustomBaseService, IInheritingService
    {

    }
}
