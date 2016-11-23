using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection.Named
{
    //public interface INamedResolutionService
    //{
    //    void ExportAs(ICompiledExportStrategy strategy, string name);

    //    object GetNamed(IExportLocatorScope scope, string name);
    //}

    //public class NamedResolutionService : INamedResolutionService
    //{
    //    private readonly IInjectionScope _scope;
    //    private ImmutableHashTree<string, IActivationStrategyCollection<ICompiledExportStrategy>> _strategies;

    //    public NamedResolutionService(IInjectionScope scope)
    //    {
    //        _scope = scope;
    //        _strategies = ImmutableHashTree<string, IActivationStrategyCollection<ICompiledExportStrategy>>.Empty;
    //    }

    //    public void ExportAs(ICompiledExportStrategy strategy, string name)
    //    {
    //        var strategyCollection = _strategies.GetValueOrDefault(name) ??
    //                                 ImmutableHashTree.ThreadSafeAdd(ref _strategies, name, new ActivationStrategyCollection<ICompiledExportStrategy>(typeof(object)));

    //        strategyCollection.AddStrategy(strategy, null);
    //    }

    //    public object GetNamed(IExportLocatorScope scope, string name)
    //    {
    //        //var strategyCollection = _strategies.GetValueOrDefault(name);

    //        //if (strategyCollection != null)
    //        //{
    //        //    var primary = strategyCollection.GetPrimary();

    //        //    if (primary != null)
    //        //    {
    //        //        var activationDelegate = primary.GetActivationStrategyDelegate(_scope, primary.InjectionScope.ActivationStrategyCompiler, primary.ActivationType);

    //        //        return activationDelegate(scope, scope, null);
    //        //    }
    //        //    else
    //        //    {
    //        //        throw new NotImplementedException();
    //        //    }
    //        //}

    //        return null;
    //    }
    //}
}
