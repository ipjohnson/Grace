using Grace.DependencyInjection.Attributes;
using Grace.Tests.Classes.Attributes;

namespace Grace.Tests.Classes.Simple
{
    public interface IKeyedMultipleService
    {
        string SomeMethod();
    }


    [ExportKeyedType(typeof(IKeyedMultipleService), "A")]
    public class KeyedMultipleServiceA : IKeyedMultipleService
    {
        public string SomeMethod()
        {
            return "A";
        }
    }

    [ExportKeyedType(typeof(IKeyedMultipleService), "B")]
    public class KeyedMultipleServiceB : IKeyedMultipleService
    {
        public string SomeMethod()
        {
            return "B";
        }
    }

    [Export]
    public class ImportKeyedMultiple
    {
        [Import(Key = "A")]
        public IKeyedMultipleService ServiceA { get; set; }

        [Import(Key = "B")]
        public IKeyedMultipleService ServiceB { get; set; }
        [AdaptedImport(Key = "A")]
        public IKeyedMultipleService AdaptedServiceA { get; set; }

        [AdaptedImport(Key = "B")]
        public IKeyedMultipleService AdaptedServiceB { get; set; }
    }
}
