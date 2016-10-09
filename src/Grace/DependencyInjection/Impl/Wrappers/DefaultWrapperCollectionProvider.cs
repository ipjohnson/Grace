namespace Grace.DependencyInjection.Impl.Wrappers
{
    public class DefaultWrapperCollectionProvider : IDefaultWrapperCollectionProvider
    {
        public IActivationStrategyCollectionContainer<ICompiledWrapperStrategy> ProvideCollection(IInjectionScope scope)
        {
            var collection = scope.ScopeConfiguration.Implementation
                    .Locate<IActivationStrategyCollectionContainer<ICompiledWrapperStrategy>>();

            collection.AddStrategy(new LazyWrapperStrategy(scope));
            collection.AddStrategy(new OwnedWrapperStrategy(scope));
            collection.AddStrategy(new MetaWrapperStrategy(scope));

            collection.AddStrategy(new FuncWrapperStrategy(scope));
            collection.AddStrategy(new FuncOneArgWrapperStrategy(scope));
            collection.AddStrategy(new FuncTwoArgWrapperStrategy(scope));
            collection.AddStrategy(new FuncThreeArgWrapperStrategy(scope));
            collection.AddStrategy(new FuncFourArgWrapperStrategy(scope));
            collection.AddStrategy(new FuncFiveArgWrapperStrategy(scope));

            return collection;
        }
    }
}
