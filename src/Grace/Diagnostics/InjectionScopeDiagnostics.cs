using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
    public class InjectionScopeDiagnostics
    {
        private readonly IInjectionScope _injectionScope;

        public InjectionScopeDiagnostics(IInjectionScope injectionScope)
        {
            _injectionScope = injectionScope;
        }

        public ExtraDataContainerDiagnostic ExtraData => new ExtraDataContainerDiagnostic(_injectionScope);


        [DebuggerDisplay("{ExportDisplayString}", Name = "Exports")]
        public ActivationStrategyCollectionContainerDiagnostic<ICompiledExportStrategy> Exports => new ActivationStrategyCollectionContainerDiagnostic<ICompiledExportStrategy>(_injectionScope.StrategyCollectionContainer);

        public ActivationStrategyCollectionContainerDiagnostic<ICompiledDecoratorStrategy> Decorators => new ActivationStrategyCollectionContainerDiagnostic<ICompiledDecoratorStrategy>(_injectionScope.DecoratorCollectionContainer);

        public ActivationStrategyCollectionContainerDiagnostic<ICompiledWrapperStrategy> Wrappers => new ActivationStrategyCollectionContainerDiagnostic<ICompiledWrapperStrategy>(_injectionScope.WrapperCollectionContainer);

        private string ExportDisplayString() => "Count: " + _injectionScope.StrategyCollectionContainer.GetAllStrategies().Count();
    }
}
