using System;
using Grace.DependencyInjection.Attributes;

namespace Grace.Tests.Classes.Attributes
{
    public interface IAttributedSingletonService
    {
        Guid UniqueId { get; }
    }

    [ExportByInterfaces]
    [Singleton]
    public class AttributedSingletonService : IAttributedSingletonService
    {
        public Guid UniqueId { get; } = Guid.NewGuid();
    }
}
