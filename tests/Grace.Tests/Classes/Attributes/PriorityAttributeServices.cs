using Grace.DependencyInjection.Attributes;

namespace Grace.Tests.Classes.Attributes
{
    public interface IPriorityAttributeService
    {

    }

    [ExportPriority(1)]
    [Export(typeof(IPriorityAttributeService))]
    public class PriorityAttributeServiceA : IPriorityAttributeService
    {
    }

    [ExportPriority(2)]
    [Export(typeof(IPriorityAttributeService))]
    public class PriorityAttributeServiceB : IPriorityAttributeService
    {
    }

    [ExportPriority(3)]
    [Export(typeof(IPriorityAttributeService))]
    public class PriorityAttributeServiceC : IPriorityAttributeService
    {
    }

    [ExportPriority(4)]
    [Export(typeof(IPriorityAttributeService))]
    public class PriorityAttributeServiceD : IPriorityAttributeService
    {
    }

    [ExportPriority(5)]
    [Export(typeof(IPriorityAttributeService))]
    public class PriorityAttributeServiceE : IPriorityAttributeService
    {
    }

}
