using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
