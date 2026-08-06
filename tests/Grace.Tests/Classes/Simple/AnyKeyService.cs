using Grace.DependencyInjection.Attributes;

namespace Grace.Tests.Classes.Simple
{
    public interface IAnyKeyService
    { }


    [ExportKeyedType(typeof(IAnyKeyService), "A")]
    public class AnyKeyServiceA : IAnyKeyService
    { }

    [ExportAnyKeyedType(typeof(IAnyKeyService))]
    public class AnyKeyServiceB : IAnyKeyService
    { }
}
