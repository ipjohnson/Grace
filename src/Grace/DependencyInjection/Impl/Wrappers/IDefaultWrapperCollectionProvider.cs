namespace Grace.DependencyInjection.Impl.Wrappers
{
    public interface IDefaultWrapperCollectionProvider
    {
        IActivationStrategyCollectionContainer<ICompiledWrapperStrategy> ProvideCollection(IInjectionScope scope);
    }
}
