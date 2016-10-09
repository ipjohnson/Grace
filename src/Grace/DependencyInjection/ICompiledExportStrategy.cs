using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    public interface ICompiledExportStrategy : IConfigurableActivationStrategy, IDecoratorOrExportActivationStrategy, IWrapperOrExportActivationStrategy
    {
        IEnumerable<ICompiledExportStrategy> SecondaryStrategies();
    }
}
