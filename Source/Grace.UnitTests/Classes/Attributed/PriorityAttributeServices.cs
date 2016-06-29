using Grace.DependencyInjection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Attributed
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
