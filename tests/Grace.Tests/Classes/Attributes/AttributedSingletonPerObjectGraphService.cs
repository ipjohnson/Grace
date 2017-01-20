using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes;

namespace Grace.Tests.Classes.Attributes
{
    [Export]
    [SingletonPerObjectGraph]
    public class AttributedSingletonPerObjectGraphService
    {
        public Guid UniqueId { get; } = Guid.NewGuid();
    }
}
