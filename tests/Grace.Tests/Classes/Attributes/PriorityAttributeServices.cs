using Grace.DependencyInjection.Attributes;

namespace Grace.Tests.Classes.Attributes
{
    public interface IPriorityAttributeService
    {

    }

    [ExportPriority(1)]
    public class PriorityAttributeServiceA : IPriorityAttributeService
    {
    }

    [ExportPriority(2)]
    public class PriorityAttributeServiceB : IPriorityAttributeService
    {
    }

    [ExportPriority(3)]
    public class PriorityAttributeServiceC : IPriorityAttributeService
    {
    }

    [ExportPriority(4)]
    public class PriorityAttributeServiceD : IPriorityAttributeService
    {
    }

    [ExportPriority(5)]
    public class PriorityAttributeServiceE : IPriorityAttributeService
    {
    }

}
