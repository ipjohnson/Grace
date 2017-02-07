using Grace.DependencyInjection.Impl.CompiledStrategies;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Provides a collection of default wrappers
    /// </summary>
    public class DefaultWrapperCollectionProvider : IDefaultWrapperCollectionProvider
    {
        /// <summary>
        /// Provide collection of wrappers
        /// </summary>
        /// <param name="scope">scope</param>
        /// <returns>wrapper collection container</returns>
        public IActivationStrategyCollectionContainer<ICompiledWrapperStrategy> ProvideCollection(IInjectionScope scope)
        {
            var collection = scope.ScopeConfiguration.Implementation
                    .Locate<IActivationStrategyCollectionContainer<ICompiledWrapperStrategy>>();

            collection.AddStrategy(new LazyWrapperStrategy(scope));
            collection.AddStrategy(new LazyMetadataWrapperStrategy(scope));
            collection.AddStrategy(new OwnedWrapperStrategy(scope));
            collection.AddStrategy(new MetaWrapperStrategy(scope));

            collection.AddStrategy(new GenericCompiledWrapperStrategy(typeof(Scoped<>), scope, scope.StrategyCompiler.DefaultStrategyExpressionBuilder) { ExternallyOwned = false });

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
