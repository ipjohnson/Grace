using Grace.DependencyInjection.Attributes;

namespace Grace.Tests.Classes.Attributes
{
    public interface IAttributedSingletonPerScopeService
    {
        
    }

    [ExportByInterfaces]
    [SingletonPerScope]
    public class AttributedSingletonPerScopeService : IAttributedSingletonPerScopeService
    {
    }
}
